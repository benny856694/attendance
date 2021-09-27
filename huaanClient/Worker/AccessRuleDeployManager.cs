using DBUtility.SQLite;
using huaanClient.Business;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using System.Data;
using System.IO;

namespace huaanClient.Worker
{
    public class AccessRuleDeployManager
    {
        private ConcurrentDictionary<int, AccessControlDeployTask> _finishedTasks = new ConcurrentDictionary<int, AccessControlDeployTask>();
        private volatile AccessControlDeployTask _currentTask;

        private static AccessRuleDeployManager _instance = null;
        private static object _locker = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _currentTaskCancellationTokenSource;
        private AutoResetEvent _event = new AutoResetEvent(false);
        private AccessRuleDeployManager()
        {

        }

        public static AccessRuleDeployManager Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new AccessRuleDeployManager();
                    }
                }

                return _instance;
            }
        }

        public Access DefaultAccess { get; set; }

        public bool CanAddTask => this._currentTask == null;

        public AccessControlDeployTask[] GetAllTasks()
        {
            var lst = new List<AccessControlDeployTask>();
            lst.AddRange(_finishedTasks.Values.ToArray());
            if (_currentTask != null)
            {
                lst.Add(_currentTask);
            }
            return lst.OrderBy(x => x.Created).ToArray();
        }

        public void LoadTasks()
        {
            AccessControlDeployTask[] tasks;
            using (var c = SQLiteHelper.GetConnection())
            {
                tasks = c.GetAll<AccessControlDeployTask>().ToArray();
            }

            foreach (var t in tasks.Where(x=>x.State == State.Finished))
            {
                _finishedTasks.TryAdd(t.Id, t);
            }

            var inprogress = tasks.FirstOrDefault(x => x.State == State.Inprogress);
            if (inprogress != null)
            {
                _currentTask = inprogress;
                _event.Set();
            }
        }

        public AccessControlDeployTask AddDeployTaskAsync()
        {
            var builder = new AccessControlDeployBuilder();
            builder.DefaultAccess = this.DefaultAccess;
            builder.Build();
            var task = new AccessControlDeployTask();
            task.Items = builder.DeployItems.ToList();
            task.TotalCount = builder.DeployItems.Length;
            task.RulesJson = JsonConvert.SerializeObject(builder.Rules);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var guid = Guid.NewGuid();
            var p = Path.Combine(appData, $"{guid}.json");
            var json = JsonConvert.SerializeObject(task.Items);
            File.WriteAllText(p, json);
            task.ItemsFilePath = p;
            
            using (var c = SQLiteHelper.GetConnection())
            {
                c.Insert(task);
            }

            _currentTask = task;
            _event.Set();
            return task;
        }

        public void removeTask(int id)
        {
            _finishedTasks.TryRemove(id, out var tsk);
            if (tsk != null)
            {
                using (var c = SQLiteHelper.GetConnection())
                {
                    c.Delete(tsk);
                }
                File.Delete(tsk.ItemsFilePath);
            }
        }

        

        public void Start()
        {
            void ProcessTask(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    if (_currentTask == null)
                    {
                        var suc = _event.WaitOne(10000);
                        if (!suc) continue;
                    }

                    if (_currentTask != null)
                    {
                        var tsk = _currentTask;
                        _currentTaskCancellationTokenSource = new CancellationTokenSource();
                        var deployer = new AccessRuleTaskDeployer(tsk);
                        deployer.DeployAsync(_currentTaskCancellationTokenSource.Token).Wait();
                        using (IDbConnection c = SQLiteHelper.GetConnection())
                        {
                            c.Update(tsk);
                        }

                        //save item state
                        var itemsJson = JsonConvert.SerializeObject(tsk.Items);
                        File.WriteAllText(tsk.ItemsFilePath, itemsJson);

                        _finishedTasks.TryAdd(tsk.Id, tsk);
                        _currentTask = null;

                    }

                }

            }

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => ProcessTask(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _event.Set();
            _cancellationTokenSource.Cancel();
            _currentTaskCancellationTokenSource.Cancel();
        }

    }
}

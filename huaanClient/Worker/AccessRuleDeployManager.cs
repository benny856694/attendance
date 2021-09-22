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

namespace huaanClient.Worker
{
    public class AccessRuleDeployManager
    {
        private ConcurrentBag<AccessControlDeployTask> _finishedTasks = new ConcurrentBag<AccessControlDeployTask>();
        private AccessControlDeployTask _currentTask;

        private static AccessRuleDeployManager _instance = null;
        private static object _locker = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _currentTaskCancellationTokenSource;
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

        public bool CanAddTask => this._currentTask == null;

        public AccessControlDeployTask[] GetAllTasks()
        {
            var lst = new List<AccessControlDeployTask>();
            lst.AddRange(_finishedTasks.ToArray());
            if (_currentTask != null)
            {
                lst.Add(_currentTask);
            }
            return lst.ToArray();
        }

        public AccessControlDeployTask AddDeployTaskAsync()
        {
            var builder = new AccessControlDeployBuilder();
            builder.Build();
            var task = new AccessControlDeployTask();
            task.RulesJson = JsonConvert.SerializeObject(builder.DeployItems);
            task.Items = builder.DeployItems.ToList();
            task.TotalCount = builder.DeployItems.Length;
            _currentTask = task;

            return task;
        }

        public void Start()
        {
            void ProcessTask(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    if (_currentTask == null)
                    {
                        Debug.WriteLine("没有规则下发任务待处理，休眠3s");
                        Thread.Sleep(3000);
                        continue;
                    }

                    var tsk = _currentTask;
                    _currentTaskCancellationTokenSource = new CancellationTokenSource();
                    Debug.WriteLine("开始处理规则下发任务");
                    var deployer = new AccessRuleTaskDeployer(tsk);
                    deployer.DeployAsync(_currentTaskCancellationTokenSource.Token).Wait();
                    _finishedTasks.Add(tsk);
                    Debug.WriteLine("规则下发处理完毕");
                    _currentTask = null;

                }

            }

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => ProcessTask(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _currentTaskCancellationTokenSource.Cancel();
        }

    }
}

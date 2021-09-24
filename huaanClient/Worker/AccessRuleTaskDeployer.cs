using huaanClient.Business;
using huaanClient.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace huaanClient.Worker
{
    public class AccessRuleTaskDeployer
    {
        private readonly AccessControlDeployTask task;
        private Dictionary<int, MyDevice> _devices;

        private object _lock = new object();

        public AccessRuleTaskDeployer(AccessControlDeployTask task)
        {
            this.task = task;
        }

        public async Task DeployAsync(CancellationToken cancellationToken)
        {
            _devices = GetData.getAllMyDevice().ToDictionary(x=>x.id);
            var deviceGroups = task.Items.GroupBy(x => x.DeviceId);
            var tsks = new List<Task>();
            foreach (var deviceGroup in deviceGroups)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (_devices.ContainsKey(deviceGroup.Key))
                    {
                        var deviceDeployer = new DeviceAccessRuleDeployer(_devices[deviceGroup.Key].ipAddress, task.RulesToDeploy.ToArray(), deviceGroup.ToArray());
                        deviceDeployer.ItemDeployedEvent += DeviceDeployer_ItemDeployedEvent;
                        deviceDeployer.RuleDeployEvent += DeviceDeployer_RuleDeployEvent;
                        var t = deviceDeployer.DeployAsync(cancellationToken);
                        tsks.Add(t);
                    }
                }
            }

            await Task.WhenAll(tsks.ToArray());
            task.State = State.Finished;
        }

        private void DeviceDeployer_RuleDeployEvent(object sender, DeployEventArgs e)
        {
            if (e.Exception != null)
            {
                var c = ((DeviceAccessRuleDeployer)sender).Items.Length;
                lock (_lock)
                {
                    task.FailCount += c;
                    task.Progress += c;

                }
            }
        }

        private void DeviceDeployer_ItemDeployedEvent(object sender, DeployEventArgs e)
        {
            lock (_lock)
            {
                if (e.Success)
                {
                    task.SuccessCount++;
                }
                else
                {
                    task.FailCount++;
                }
                task.Progress++;

            }
        }
    }
}

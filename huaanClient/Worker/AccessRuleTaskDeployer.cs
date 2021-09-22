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

        public AccessRuleTaskDeployer(AccessControlDeployTask task)
        {
            this.task = task;
        }

        public async Task DeployAsync(CancellationToken cancellationToken)
        {
            _devices = GetData.getAllMyDevice().ToDictionary(x=>x.id);
            var deviceGroups = task.Items.GroupBy(x => x.DeviceId);
            foreach (var deviceGroup in deviceGroups)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    var deviceDeployer = new DeviceAccessRuleDeployer(_devices[deviceGroup.Key].ipAddress, task.RulesToDeploy.ToArray(), deviceGroup.ToArray());
                    deviceDeployer.ItemDeployedEvent += DeviceDeployer_ItemDeployedEvent;
                    await deviceDeployer.DeployAsync(cancellationToken);
                }
            }
            task.State = State.Finished;
        }

        private void DeviceDeployer_ItemDeployedEvent(object sender, ItemDeployedEventArgs e)
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

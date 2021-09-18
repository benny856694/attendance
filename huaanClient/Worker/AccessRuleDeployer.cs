using huaanClient.Business;
using huaanClient.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Worker
{
    public  class AccessRuleDeployer
    {
        private readonly AccessControlDeployRule[] rules;
        private readonly AccessControlDeployItem[] items;
        private Dictionary<int, MyDevice> _devices;

        public AccessRuleDeployer(AccessControlDeployRule[] rules, AccessControlDeployItem[] items)
        {
            this.rules = rules;
            this.items = items;
        }

        public async Task DeployAsync()
        {
            _devices = GetData.getAllMyDevice().ToDictionary(x=>x.id);
            var deviceGroups = items.GroupBy(x => x.DeviceId);
            foreach (var deviceGroup in deviceGroups)
            {
                var deviceDeployer = new DeviceAccessRuleDeployer(_devices[deviceGroup.Key].ipAddress, rules, deviceGroup.ToArray());
                await deviceDeployer.DeployAsync();
            }
        }
    }
}

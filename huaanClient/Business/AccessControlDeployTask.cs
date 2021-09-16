using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Business
{
    internal class AccessControlDeployTask
    {
        public DateTime Created {  get; set; }
        public List<AccessControlDeployRule> RulesToDeploy { get; set; } = new List<AccessControlDeployRule>();
        public List<AccessControlDeployItem> Items {  get; set; } = new List<AccessControlDeployItem>();
    }
}

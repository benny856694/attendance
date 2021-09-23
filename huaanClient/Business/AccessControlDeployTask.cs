using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Business
{
    [Table("AccessControlDeployTask")]
    public class AccessControlDeployTask
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created {  get; set; } = DateTime.Now;
        public State State { get; set; } = State.Inprogress;
        public int TotalCount {  get; set; }
        public int DeviceCount { get; set;  }
        public int Progress { get; set; }
        public int FailCount { get; set; }
        public int SuccessCount { get; set; }

        public string ItemsFilePath { get; set; }
        public string RulesJson { get; set; }
        
        
        [Computed]
        public List<AccessControlDeployRule> RulesToDeploy { get; set; } = new List<AccessControlDeployRule>();
        [Computed]
        public List<AccessControlDeployItem> Items {  get; set; } = new List<AccessControlDeployItem>();
    }
}

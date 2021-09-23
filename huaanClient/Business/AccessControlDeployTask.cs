using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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


        private List<AccessControlDeployRule> _rules;
        private List<AccessControlDeployItem> _items;
        private object _ruleLock = new object();
        private object _itemLock = new object();

        [Computed]
        public List<AccessControlDeployRule> RulesToDeploy 
        { 
            get
            {
                lock (this._ruleLock)
                {
                    if (_rules == null)
                    {
                        if (!string.IsNullOrEmpty(this.RulesJson))
                        {
                            _rules = JsonConvert.DeserializeObject<List<AccessControlDeployRule>>(this.RulesJson);
                        }
                        else
                        {
                            _rules = new List<AccessControlDeployRule>();
                        }
                    }
                }

                return this._rules;
            }
            set
            {
                lock (this._ruleLock)
                {
                    this._rules = value;
                }
            }
        }
        [Computed]
        public List<AccessControlDeployItem> Items 
        {  
            get
            {
                lock (this._itemLock)
                {
                    if (_items == null)
                    {
                        if (File.Exists(this.ItemsFilePath))
                        {
                            var json = File.ReadAllText(this.ItemsFilePath);
                            this._items = JsonConvert.DeserializeObject<List<AccessControlDeployItem>>(json);
                        }
                        else
                        {
                            this._items = new List<AccessControlDeployItem>();
                        }
                    }

                    return this._items;

                }
            }
            set
            {
                lock(this._itemLock)
                {
                    this._items = value;
                }
            }
        } 
    }
}

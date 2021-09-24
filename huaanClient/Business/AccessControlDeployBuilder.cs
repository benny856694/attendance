using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace huaanClient.Business
{
    public class AccessControlDeployBuilder
    {
        private Dictionary<(string, int), AccessControlDeployItem> _items =
            new Dictionary<(string, int), AccessControlDeployItem>();
        private AccessControlDeployRule[] _rules = new AccessControlDeployRule[0];


        public AccessControlDeployRule[] Rules => _rules;
        public AccessControlDeployItem[] DeployItems => this._items.Values.ToArray();

        public void  Build()
        {
            var rules = GetData.GetAllAccessRules();
            var ruleIdToRuleMap = rules.ToDictionary(x => x.Id);
            BuildRules(rules);
            var distributions = GetData.GetAllRuleDistribution();
            var allStaffs = GetData.GetAllStaffs();
            BuildItems(distributions, ruleIdToRuleMap, allStaffs);

        }

        private void BuildItems(
            Database.RuleDistribution[] distributions,
            Dictionary<int, Database.AccessRule> ruleIdToRuleMap,
            Database.Staff[] allStaffs)
        {
            var distributionCategories = distributions.GroupBy(x => x.DistributionItemType);
            var employeeTypeDistribution = distributionCategories
                .FirstOrDefault(x => x.Key == Database.DistributionItemType.EmployeeType);
            var departmentDistribution = distributionCategories
                .FirstOrDefault(x => x.Key == Database.DistributionItemType.Department);
            var staffDistribution = distributionCategories
                .FirstOrDefault(x => x.Key == Database.DistributionItemType.Staff);

            BuildGroupItems(employeeTypeDistribution, ruleIdToRuleMap, allStaffs);
            BuildGroupItems(departmentDistribution, ruleIdToRuleMap, allStaffs);
            BuildStaffItems(staffDistribution, ruleIdToRuleMap, allStaffs);
            
        }

        private void BuildStaffItems(IGrouping<Database.DistributionItemType, Database.RuleDistribution> staffDistribution, Dictionary<int, Database.AccessRule> ruleIdToRuleMap, Database.Staff[] allStaffs)
        {
            if (staffDistribution == null) return;

            var orderedStaffDistribution = staffDistribution.Where(x=>x.AccessRuleId != null).OrderBy(x => x.Priority);
            foreach (var distribution in orderedStaffDistribution)
            {
                foreach (var staffItem in distribution.Items)
                {
                    foreach (var dev in distribution.Devices)
                    {
                        var deployItem = new AccessControlDeployItem()
                        {
                            id = staffItem.StaffId,
                            DeviceId = dev.DeviceId,
                            kind = ruleIdToRuleMap[distribution.AccessRuleId.Value].Index,
                        };
                        _items[deployItem.Key] = deployItem;
                    }
                }
            }
        }

        private void BuildGroupItems(IGrouping<Database.DistributionItemType, Database.RuleDistribution> groupDistributions, Dictionary<int, Database.AccessRule> ruleIdToRuleMap, Database.Staff[] allStaffs)
        {
            if (groupDistributions == null) return;

            var orderedDistribution = groupDistributions.Where(x=>x.AccessRuleId != null).OrderBy(x => x.Priority);
            foreach (var distribution in orderedDistribution)
            {
                foreach (var group in distribution.Items)
                {
                    var staffsOfSpecifiedGroup = allStaffs.Where(x => 
                        SelectGroupIdFromStaff(group.GroupType, x) == group.GroupId);

                    foreach (var dev in distribution.Devices)
                    {
                        foreach (var staff in staffsOfSpecifiedGroup)
                        {
                            var deployItem = new AccessControlDeployItem()
                            {
                                id = staff.id,
                                DeviceId = dev.DeviceId,
                                kind = ruleIdToRuleMap[distribution.AccessRuleId.Value].Index,
                            };
                            _items[deployItem.Key] = deployItem;
                        }
                        
                    }
                }
                
            }
        }

        private int SelectGroupIdFromStaff(Database.GroupIdType groupType, Database.Staff staff)
        {
            var groupId = -1;
            switch (groupType)
            {
                case Database.GroupIdType.EmployeeType:
                    groupId = staff.Employetype_id;
                    break;
                case Database.GroupIdType.Department:
                    groupId = staff.department_id;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return groupId;
        }

        private  void BuildRules(Database.AccessRule[] rules)
        {
            var deployRules = new List<AccessControlDeployRule>();
            for (int i = 0; i < rules.Length; i++)
            {
                var rule = rules[i];
                rule.Index = i + 1;
                var deployRule = new AccessControlDeployRule()
                {
                    name = rule.Name,
                    kind = rule.Index,
                    mode = rule.RepeatType == Database.RepeatType.RepeatByDay ? "daily" : "weekly"
                };

                foreach (var day in rule.Days.OrderBy(x => x.DayOfWeek).Take(rule.RepeatType == Database.RepeatType.RepeatByWeek ? 7 : 1))
                {
                    var deployDay = new AccessControlDeplyDay();
                    foreach (var segment in day.TimeSegments)
                    {
                        var start = ConvertTimeSegment(segment.Start);
                        var end = ConvertTimeSegment(segment.End);

                        var section = new AccessControlDeploySection();
                        section.start = start;
                        section.end = end;

                        deployDay.sections.Add(section);
                    }
                    deployRule.days.Add(deployDay);
                }

                deployRules.Add(deployRule);
            }

            _rules = deployRules.ToArray();
        }

        private static AccessControlDeployHourMinute ConvertTimeSegment(string segment)
        {
            var hm = segment.toHourMinute();
            var deployHm = new AccessControlDeployHourMinute();
            deployHm.hour = hm.hour;
            deployHm.minute = hm.minute;
            return deployHm;
        }
    }
}

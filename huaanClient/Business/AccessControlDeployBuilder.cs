using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace huaanClient.Business
{
    internal static class AccessControlDeployBuilder
    {
        public static (AccessControlDeployItem[] items, AccessControlDeployRule[] rules) Build()
        {
            var rules = GetData.GetAllAccessRules();
            var ruleIdToRuleMap = rules.ToDictionary(x => x.Id);
            var deployRules = BuildRules(rules);
            //var deployItems = BuildItems(distributions);

            return (null, deployRules.ToArray());

        }

        private static object BuildItems(Database.RuleDistribution[] distributions)
        {
            throw new NotImplementedException();
        }

        private static List<AccessControlDeployRule> BuildRules(Database.AccessRule[] rules)
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

            return deployRules;
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

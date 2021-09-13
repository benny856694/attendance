using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
	[Table("RuleDistribution")]
	public class RuleDistribution : IEquatable<RuleDistribution>
	{
		[Key]
		public int Id { get; set; }
		public string Name {  get; set; }
		public int AccessRuleId { get; set; }
		public int Priority { get; set; }
		[Computed]
		public List<RuleDistributionDevice> Devices {  get; set; }
		[Computed]
		public List<RuleDistributionItem> Items {  get; set; }


		public override bool Equals(object obj) => this.Equals(obj as RuleDistribution);
        public bool Equals(RuleDistribution r)
        {
            if (r is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, r))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != r.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (Id == r.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}

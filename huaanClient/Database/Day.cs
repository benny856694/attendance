using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Database
{
    [Table("Day")]
    public class Day : IEquatable<Day>
    {
        [Key]
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int AccessRuleId { get; set; }

        [Computed]
        public List<TimeSegment> TimeSegments { get; set; }

        public bool Equals(Day d)
        {
            if (d is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, d))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != d.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return Id == d.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}

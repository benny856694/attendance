using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Business
{
    public class AccessControlDeployItem : IEquatable<AccessControlDeployItem>
    {
        public string id {  get; set; }
        public int kind { get; set; }
        public int DeviceId { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public (string, int) Key => (this.id, this.DeviceId);

        public override bool Equals(object obj)
        {
            return obj is AccessControlDeployItem item &&
                   id == item.id &&
                   DeviceId == item.DeviceId;
        }

        public bool Equals(AccessControlDeployItem other)
        {
            if (other is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (id == other.id && DeviceId == other.DeviceId);
        }

        public override int GetHashCode()
        {
            int hashCode = 1025083102;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + DeviceId.GetHashCode();
            return hashCode;
        }
    }
}

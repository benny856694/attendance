using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Model
{
    public class Device : IEquatable<Device>
    {
        public string Name { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return Name == null ? IP : $"{Name} | {IP}";
        }

        public override bool Equals(object obj) => this.Equals(obj as Device);
       

        public bool Equals(Device other)
        {
            if (other is null) return false;

            if (object.ReferenceEquals(this, other)) return true;

            if (this.GetType() != other.GetType()) return false;


            return this.IP == other.IP;
        }
    }
}

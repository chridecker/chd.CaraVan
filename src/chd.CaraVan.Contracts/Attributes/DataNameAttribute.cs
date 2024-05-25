using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Attributes
{
    public class DataNameAttribute : Attribute
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public DataNameAttribute(string name, string unit)
        {
            this.Name = name;
            this.Unit = unit;
        }
    }
}

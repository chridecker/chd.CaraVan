using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.DataAccess
{
    public class InfluxSettings
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }
        public string Database { get; set; }
        public string Org { get; set; }
    }
}

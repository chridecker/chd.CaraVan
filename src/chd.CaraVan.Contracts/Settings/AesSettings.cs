using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Settings
{
    public class AesSettings
    {
        public decimal? BatteryLimit { get; set; }
        public decimal? SolarAmpLimit { get; set; }
        public TimeSpan? AesTimeout { get; set; }
    }
}

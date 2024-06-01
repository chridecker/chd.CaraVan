using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Constants
{
    public class SolarStateConstants
    {
        public const string STANDBY = "StandBy";
        public const string ACTIVE = "Aktiv";
        public const string REDUCE = "Stromreduzierung";

        public const decimal STANDBY_STATE = 0;
        public const decimal ACTIVE_STATE = 9;
        public const decimal REDUCE_STATE = 25;
        public const decimal REDUCE_STATE2 = 57;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Enums
{
    public enum EDataType
    {
        None = 0,
        Temperature = 1,
        Humidity = 2,
        Pressure = 3,
        BatteryLevel = 4,
        BatteryLevelAh = 5,
        Ampere = 6,
        WattH = 7,
        BatteryPercent = 8,
        BatteryLoadingState = 9
    }
}

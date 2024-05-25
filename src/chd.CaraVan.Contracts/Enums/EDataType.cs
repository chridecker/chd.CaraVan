using chd.CaraVan.Contracts.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Enums
{
    public enum EDataType
    {
        None= 0,
        [DataName("Temperatur","°C")]
        Temperature = 1,
        [DataName("Luftfeuchtigkeit", "%")]
        Humidity = 2,
        [DataName("Luftdruck", "hPa")]
        Pressure = 3,
        [DataName("Spannung", "V")]
        BatteryLevel = 4,
        [DataName("Ladung", "Ah")]
        BatteryLevelAh = 5,
        [DataName("Akutell", "A")]
        Ampere = 6,
        [DataName("Leistung", "W")]
        Watt = 7
    }
}

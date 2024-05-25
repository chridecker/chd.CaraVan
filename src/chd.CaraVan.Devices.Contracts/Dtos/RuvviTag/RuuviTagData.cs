using chd.CaraVan.Devices.Contracts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.RuvviTag
{
    public class RuuviTagData
    {
        private readonly byte[] _data;

        public decimal? Temperature => this.GetTemperature(this._data);
        public decimal? Humidity => this.GetHumidity(this._data);
        public decimal? Pressure => this.GetPressure(this._data);
        public decimal? BatteryLevel => this.GetBatteryLevel(this._data);
        public RuuviTagData(byte[] data)
        {
            this._data = data;
        }

        private decimal? GetTemperature(byte[] data)
        {
            var hex = Convert.ToHexString(data.Skip(1).Take(2).ToArray());
            var val = Convert.ToInt16(hex, 16);
            if (val == -32768) { return null; }
            return Math.Round(val / 200m, 2);

        }
        private decimal? GetHumidity(byte[] data)
        {
            var hex = Convert.ToHexString(data.Skip(3).Take(2).ToArray());
            var val = Convert.ToInt16(hex, 16);
            if (val == 65535) { return null; }
            return Math.Round(val / 400m, 2);
        }
        private decimal? GetPressure(byte[] data)
        {
            var hex = Convert.ToHexString(data.Skip(5).Take(2).ToArray());
            var val = Convert.ToInt16(hex, 16);
            if (val == 0xFFFF) { return null; }
            return Math.Round((val + 5000) / 100m, 2);
        }
        private decimal? GetBatteryLevel(byte[] data)
        {
            var hex = Convert.ToHexString(data.Skip(13).Take(2).ToArray());
            var val = Convert.ToInt16(hex, 16);
            if (val + 1600 > 3647) { return null; }
            return Math.Round((val + 1600) / 1000m, 2);
        }
    }
}

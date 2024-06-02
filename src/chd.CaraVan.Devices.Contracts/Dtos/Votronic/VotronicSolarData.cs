using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.Votronic
{
    public class VotronicSolarData : VotronicData
    {


        public VotronicSolarData(byte[] data) : base(data)
        {
        }

        public decimal Ampere => this.GetData(4, 2, 10m);

        public decimal WattH => this.GetData(15, 2, 0.1m);
        public decimal AH => this.GetData(13, 2, 1m);

        public byte State => this._data[12];
    }
}

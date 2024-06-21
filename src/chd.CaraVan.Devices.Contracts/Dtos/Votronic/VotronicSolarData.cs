using System;
using System.Collections;
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
        public decimal VoltageSolar => this.GetData(2, 2, 100m);
        public decimal Ampere => this.GetData(4, 2, 10m);

        public decimal WattH => this.GetData(15, 2, 0.1m);
        public decimal AH => this.GetData(13, 2, 1m);

        public byte State => this._data[12];
        public byte LoadingState => this._data[11];

        public string LoadingPhase => (this.Active, this.LoadingState) switch
        {
            (true, 0) => "I-Phase",
            (true, 1) => "U1-Phase",
            (true, 2) => "U2-Phase",
            (true, 3) => "U3-Phase",
            (true, _) => this.LoadingState.ToString(),
            (false, _) => "",
        };

        public bool Active => new BitArray(new byte[] { this.State })[3];
        public bool Reduce => new BitArray(new byte[] { this.State })[4];
        public bool AES => new BitArray(new byte[] { this.State })[5];
    }
}

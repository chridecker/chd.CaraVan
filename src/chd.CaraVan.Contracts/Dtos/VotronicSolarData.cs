using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class VotronicSolarData : VotronicData
    {
        public byte State { get; set; }
        public string LoadingPhase {get;set;}
        public decimal WattH { get; set; }
        public decimal VoltageSolar { get; set; }

        public bool Active => new BitArray(new byte[] { State })[3];
        public bool Reduce => new BitArray(new byte[] { State })[4];
        public bool AES => new BitArray(new byte[] { State })[5];
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.Victron
{
    public class VictronData
    {
        private readonly byte[] data;
    
        public VictronData(byte[] data)
        {
            this.data = data;
        }
        public byte Type => data[0];
        public byte State => data[5];
        public byte Error => data[6];
        public decimal Ampere
        {
            get
            {
                var b = new BitArray(data.Skip(6).Take(2).ToArray());
                var newB = new BitArray(11);
                for (int i = 0; i < newB.Length; i++)
                {
                    newB[i] = b[i];
                }
                int[] amp = new int[1];
                newB.CopyTo(amp, 0);
                return amp[0] * 0.1m;
            }
        }
        public decimal AmpereAC
        {
            get
            {
                var b = new BitArray(data.Skip(16).Take(2).ToArray());
                var newB = new BitArray(9);
                for (int i = 0; i < newB.Length; i++)
                {
                    newB[i] = b[i];
                }
                int[] amp = new int[1];
                newB.CopyTo(amp, 0);
                return amp[0] * 0.1m;
            }
        }
    }            
}

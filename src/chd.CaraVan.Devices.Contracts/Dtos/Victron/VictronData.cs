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
        public byte State => data[4];
        public byte Error => data[5];
        public decimal Ampere
        {
            get
            {
                var bitVolt = 13;
                var b = new BitArray(data.Skip(6).Take(3).ToArray());
                var newB = new BitArray(11);
                for (int i = 0; i < b.Length; i++)
                {
                    if(i <bitVolt)
                    {
                    }
                    else {
                    newB[i-bitVolt] = b[i];
                    }
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
                var bitTemp = 7;
                var b = new BitArray(data.Skip(15).Take(2).ToArray());
                var newB = new BitArray(9);
                for (int i = 0; i < b.Length; i++)
                {
                    if(i<bitTemp){
                    }
                    else{
                    newB[i-bitTemp] = b[i];
                    }
                }
                int[] amp = new int[1];
                newB.CopyTo(amp, 0);
                return amp[0] * 0.1m;
            }
        }
    }            
}

using chd.CaraVan.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Settings
{
    public class DeviceSettings
    {
        public IEnumerable<RuuviDeviceDto> RuuviTags { get; set; }
        public VotronicDto Votronic{ get; set; }
    }
}

using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Devices.Interfaces
{
    public interface IRuuviITag : IDevice
    {
        Task<RuuviTagData> GetDataAsync(CancellationToken cancellationToken = default);
}
}

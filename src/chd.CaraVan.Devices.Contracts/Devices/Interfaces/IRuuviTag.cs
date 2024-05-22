using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Devices.Interfaces
{
    public interface IRuuviTag : IDevice
    {
        Task<RuuviTagData> GetDataAsync(CancellationToken cancellationToken = default);
        event EventHandler<RuuviTagEventArgs> DataReceived;
    }
}

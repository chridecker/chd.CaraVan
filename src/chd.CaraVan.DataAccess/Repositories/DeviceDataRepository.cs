using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.DataAccess.Repositories
{
    public class DeviceDataRepository : BaseDataRepository<DeviceData, int>, IDeviceDataRepository
    {
        public async Task<DeviceData> GetLatestAsync(int device, EDataType type, CancellationToken cancellationToken = default)
        => this._bag.Where(x => x.DeviceId == device && x.Type == type).OrderByDescending(o => o.RecordDateTime).FirstOrDefault();

    }
    public interface IDeviceDataRepository : IDataRepository<DeviceData, int>
    {
        Task<DeviceData> GetLatestAsync(int device, EDataType type, CancellationToken cancellationToken = default);
    }
}

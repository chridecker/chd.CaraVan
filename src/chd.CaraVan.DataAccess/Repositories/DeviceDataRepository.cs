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
    public class DeviceDataRepository : BaseDataRepository<DeviceData>, IDeviceDataRepository
    {
        public DeviceData GetLatest(int device, EDataType type) => this._lst.Where(x => x.DeviceId == device && x.Type == type).OrderByDescending(o => o.RecordDateTime).FirstOrDefault();

    }
    public interface IDeviceDataRepository : IDataRepository<DeviceData>
    {
        DeviceData GetLatest(int device, EDataType type);
    }
}

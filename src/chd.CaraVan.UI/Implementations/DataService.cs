using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace chd.CaraVan.UI.Implementations
{
    public class DataService : IDataService
    {
        private readonly IDeviceDataRepository _deviceDataRepository;

        public DataService(IDeviceDataRepository deviceDataRepository)
        {
            this._deviceDataRepository = deviceDataRepository;
        }
        public async Task<ICollection<DeviceData>> GetDeviceDataAsync(int deviceId, EDataType type, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            var entries = await this._deviceDataRepository.GetAsync(type, from, to, cancellationToken);
            return entries.Where(x => x.DeviceId == deviceId).ToList();
        }
        public Task<DeviceData> GetLatestDataToDeviceAsync(int device, EDataType type, CancellationToken cancellationToken = default)
            => this._deviceDataRepository.GetLatestAsync(device, type, cancellationToken);
    }
    public interface IDataService
    {
        Task<ICollection<DeviceData>> GetDeviceDataAsync(int deviceId, EDataType type, DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<DeviceData> GetLatestDataToDeviceAsync(int device, EDataType type, CancellationToken cancellationToken = default);
    }
}

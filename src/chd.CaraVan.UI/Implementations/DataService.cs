using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class DataService : IDataService
    {
        public async Task<ICollection<DeviceData>> GetDeviceDataAsync(int deviceId, EDataType type, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            var lst = new List<DeviceData>();
            var entries = (to - from).TotalMinutes;
            var rand = new Random();
            for (var i = 0; i < entries; i++)
            {
                lst.Add(new(i + 1, DateTime.Now.AddMinutes((i - entries)), type, rand.Next(18,type== EDataType.Temperature ? 36 : 95 ))
                {
                    DeviceId = deviceId
                });
            }
            return lst;
        }
    }
    public interface IDataService
    {
        Task<ICollection<DeviceData>> GetDeviceDataAsync(int deviceId, EDataType type, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}

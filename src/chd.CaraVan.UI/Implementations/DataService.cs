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
        public async Task<ICollection<IData<int>>> GetDeviceDataAsync(int deviceId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            var lst = new List<IData<int>>();
            var entries = (to - from).TotalMinutes;
            var rand = new Random(200);
            for (var i = 0; i < entries; i++)
            {
                //await Task.Delay(1, cancellationToken);
                lst.Add(new DeviceData(i + 1, DateTime.Now.AddMinutes((i - entries)), EDataType.Temperature, rand.Next())
                {
                    DeviceId = deviceId
                });
            }
            return lst;
        }
    }
    public interface IDataService
    {
        Task<ICollection<IData<int>>> GetDeviceDataAsync(int deviceId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}

using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace chd.CaraVan.UI.Implementations
{
    public class RuuviTagDataService : IRuuviTagDataService
    {
        private readonly IInfluxContext _influxContext;
        private readonly ConcurrentDictionary<int, RuuviTagDeviceData> _dict;

        public RuuviTagDataService(IInfluxContext influxContext)
        {
            this._influxContext = influxContext;
            this._dict = new ConcurrentDictionary<int, RuuviTagDeviceData>();
        }
        public async Task AddData(int id, RuuviTagDeviceData data)
        {
            await this._influxContext.WriteSensorData(data.RecordDateTime, id, data.Value);
        }

        public Task<RuuviTagDeviceData> GetData(int id) => Task.FromResult(this._dict[id]);

    }
    public interface IRuuviTagDataService
    {
        Task AddData(int id, RuuviTagDeviceData data);
        Task<RuuviTagDeviceData> GetData(int id);
    }
}

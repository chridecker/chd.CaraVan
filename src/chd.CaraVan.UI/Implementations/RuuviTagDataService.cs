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

        public RuuviTagDataService(IInfluxContext influxContext)
        {
            this._influxContext = influxContext;
        }
        public async Task AddData(int id, RuuviTagDeviceData data)
        {
            await this._influxContext.WriteSensorData(data.RecordDateTime, id, data.Value);
        }


        public RuuviTagDeviceData GetData(int id, EDataType type)
        {
           
        }

    }
    public interface IRuuviTagDataService
    {
        Task AddData(int id, RuuviTagDeviceData data);
        RuuviTagDeviceData GetData(int id, EDataType type);
    }
}

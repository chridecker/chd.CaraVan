using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
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
    public class DataService : IDataService
    {
        private readonly ConcurrentDictionary<int, IDictionary<EDataType, DeviceData>> _dataDict;

        public DataService()
        {
            this._dataDict = new ConcurrentDictionary<int, IDictionary<EDataType, DeviceData>>();
        }
        public void AddData(int id, DeviceData data)
        {
            if (!this._dataDict.ContainsKey(id)) { this._dataDict[id] = new Dictionary<EDataType, DeviceData>(); }
            this._dataDict[id][data.Type] = data;
        }
        public DeviceData GetData(int id, EDataType type)
        {
            if (this._dataDict.TryGetValue(id, out var dic)
                && dic.TryGetValue(type, out var val))
            {
                return val;
            }
            return null;
        }
    }
    public interface IDataService
    {
        void AddData(int id, DeviceData data);
        DeviceData GetData(int id, EDataType type);
    }
}

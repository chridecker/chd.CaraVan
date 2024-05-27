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
    public class RuuviTagDataService : IRuuviTagDataService
    {
        private readonly ConcurrentDictionary<int, IDictionary<EDataType, RuuviTagDeviceData>> _dataDict;
        private readonly ConcurrentDictionary<int, IDictionary<DateTime, IDictionary<EDataType, (decimal? min, decimal? max)>>> _minMaxDataDict;

        public RuuviTagDataService()
        {
            this._dataDict = new ConcurrentDictionary<int, IDictionary<EDataType, RuuviTagDeviceData>>();
            this._minMaxDataDict = new ConcurrentDictionary<int, IDictionary<DateTime, IDictionary<EDataType, (decimal? min, decimal? max)>>>();
        }
        public void AddData(int id, RuuviTagDeviceData data)
        {
            if (!this._dataDict.ContainsKey(id)) { this._dataDict[id] = new Dictionary<EDataType, RuuviTagDeviceData>(); }
            this._dataDict[id][data.Type] = data;
        }
        public RuuviTagDeviceData GetData(int id, EDataType type)
        {
            if (this._dataDict.TryGetValue(id, out var dic)
                && dic.TryGetValue(type, out var val))
            {
                return val;
            }
            return null;
        }
        private void HandleMinMax(int id, RuuviTagDeviceData data)
        {
            if (!this._minMaxDataDict.ContainsKey(id)) { this._minMaxDataDict[id] = new Dictionary<DateTime, IDictionary<EDataType, (decimal?, decimal?)>>(); }
            if (this._minMaxDataDict[id].ContainsKey(data.RecordDateTime.Date)) { this._minMaxDataDict[id][data.RecordDateTime.Date] = new Dictionary<EDataType, (decimal?, decimal?)>(); }
            if (this._minMaxDataDict[id].ContainsKey(data.RecordDateTime.AddDays(-1).Date)) { this._minMaxDataDict[id].Remove(data.RecordDateTime.AddDays(-1).Date); }
            if (!this._minMaxDataDict[id][data.RecordDateTime.Date].ContainsKey(data.Type)){this._minMaxDataDict[id][data.RecordDateTime.Date][data.Type] = (null, null);}

            if (!this._minMaxDataDict[id][data.RecordDateTime.Date][data.Type].min.HasValue
                || this._minMaxDataDict[id][data.RecordDateTime.Date][data.Type].min.Value > data.Value)
            {
                this._minMaxDataDict[id][data.RecordDateTime.Date][data.Type].min = data.Value;
            }
            if (!this._minMaxDataDict[id][data.RecordDateTime.Date][data.Type].min.HasValue
                || this._minMaxDataDict[id][data.RecordDateTime.Date][data.Type].min.Value > data.Value)
            {
                this._minMaxDataDict[id][data.RecordDateTime.Date][data.Type].min = data.Value;
            }
        }


    }
    public interface IRuuviTagDataService
    {
        void AddData(int id, RuuviTagDeviceData data);
        RuuviTagDeviceData GetData(int id, EDataType type);
    }
}

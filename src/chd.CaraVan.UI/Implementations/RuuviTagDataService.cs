using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.Contracts.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace chd.CaraVan.UI.Implementations
{
    public class RuuviTagDataService : IRuuviTagDataService
    {
        private readonly ConcurrentDictionary<int, IDictionary<EDataType, RuuviTagDeviceData>> _dataDict;
        private readonly ConcurrentDictionary<int, IDictionary<DateTime, IDictionary<EDataType, decimal?>>> _minDataDict;
        private readonly ConcurrentDictionary<int, IDictionary<DateTime, IDictionary<EDataType, decimal?>>> _maxDataDict;
        private readonly IOptionsMonitor<DeviceSettings> _optionsMonitor;

        public RuuviTagDataService(IOptionsMonitor<DeviceSettings> optionsMonitor)
        {
            this._dataDict = new ConcurrentDictionary<int, IDictionary<EDataType, RuuviTagDeviceData>>();
            this._minDataDict = new ConcurrentDictionary<int, IDictionary<DateTime, IDictionary<EDataType, decimal?>>>();
            this._maxDataDict = new ConcurrentDictionary<int, IDictionary<DateTime, IDictionary<EDataType, decimal?>>>();
            this._optionsMonitor = optionsMonitor;
        }
        public Task<IEnumerable<RuuviDeviceDto>> Devices => Task.FromResult(this._optionsMonitor.CurrentValue.RuuviTags);

        public Task AddData(int id, RuuviTagDeviceData data, CancellationToken cancellationToken = default) => Task.Run(()=>
        {
            if (!this._dataDict.ContainsKey(id)) { this._dataDict[id] = new Dictionary<EDataType, RuuviTagDeviceData>(); }
            this._dataDict[id][data.Type] = data;
            this.HandleMinMax(id, data);
        },cancellationToken);

        private (decimal? Min, decimal? Max) GetMinMaxData(int id, EDataType type)
        {
            decimal? min = null;
            decimal? max = null;

            if (this._minDataDict.TryGetValue(id, out var minVal))
            {
                if (minVal.TryGetValue(DateTime.Now.Date, out var data))
                {
                    data.TryGetValue(type, out min);
                }
            }
            if (this._maxDataDict.TryGetValue(id, out var maxVal))
            {
                if (maxVal.TryGetValue(DateTime.Now.Date, out var data))
                {
                    data.TryGetValue(type, out max);
                }
            }
            return (min, max);
        }

        public async Task<RuuviSensorDataDto> GetData(int id, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            if (this._dataDict.TryGetValue(id, out var dic)
                && dic.TryGetValue(EDataType.Temperature, out var val))
            {
                var minMax = this.GetMinMaxData(id, EDataType.Temperature);
                return new RuuviSensorDataDto
                {
                    Value = val.Value,
                    Record = val.RecordDateTime,
                    Id = id,
                    Max = minMax.Max,
                    Min = minMax.Min
                };
            }
            return null;
        }

        private void HandleMinMax(int id, RuuviTagDeviceData data)
        {
            if (!this._minDataDict.ContainsKey(id)) { this._minDataDict[id] = new Dictionary<DateTime, IDictionary<EDataType, decimal?>>(); }
            if (!this._maxDataDict.ContainsKey(id)) { this._maxDataDict[id] = new Dictionary<DateTime, IDictionary<EDataType, decimal?>>(); }

            if (!this._minDataDict[id].ContainsKey(data.RecordDateTime.Date)) { this._minDataDict[id][data.RecordDateTime.Date] = new Dictionary<EDataType, decimal?>(); }
            if (!this._maxDataDict[id].ContainsKey(data.RecordDateTime.Date)) { this._maxDataDict[id][data.RecordDateTime.Date] = new Dictionary<EDataType, decimal?>(); }

            if (!this._minDataDict[id][data.RecordDateTime.Date].ContainsKey(data.Type)) { this._minDataDict[id][data.RecordDateTime.Date][data.Type] = null; }
            if (!this._maxDataDict[id][data.RecordDateTime.Date].ContainsKey(data.Type)) { this._maxDataDict[id][data.RecordDateTime.Date][data.Type] = null; }

            if (this._minDataDict[id].ContainsKey(data.RecordDateTime.AddDays(-1).Date)) { this._minDataDict[id].Remove(data.RecordDateTime.AddDays(-1).Date); }
            if (this._maxDataDict[id].ContainsKey(data.RecordDateTime.AddDays(-1).Date)) { this._maxDataDict[id].Remove(data.RecordDateTime.AddDays(-1).Date); }

            if (!this._minDataDict[id][data.RecordDateTime.Date][data.Type].HasValue
                || this._minDataDict[id][data.RecordDateTime.Date][data.Type].Value > data.Value)
            {
                this._minDataDict[id][data.RecordDateTime.Date][data.Type] = data.Value;
            }
            if (!this._maxDataDict[id][data.RecordDateTime.Date][data.Type].HasValue
                || this._maxDataDict[id][data.RecordDateTime.Date][data.Type].Value < data.Value)
            {
                this._maxDataDict[id][data.RecordDateTime.Date][data.Type] = data.Value;
            }
        }
    }
    public interface IRuuviTagDataService
    {
        Task<IEnumerable<RuuviDeviceDto>> Devices { get; }

        Task AddData(int id, RuuviTagDeviceData data, CancellationToken cancellationToken = default);
        Task<RuuviSensorDataDto> GetData(int id, CancellationToken cancellationToken = default);
    }
}

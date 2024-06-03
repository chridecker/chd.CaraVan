using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.DataAccess
{
    public class InfluxContext : IInfluxContext
    {
        private readonly InfluxDBClient _client;
        private readonly IOptionsMonitor<InfluxSettings> _optionsMonitor;

        public InfluxContext(IOptionsMonitor<InfluxSettings> optionsMonitor)
        {
            this._optionsMonitor = optionsMonitor;

            this._client = new InfluxDBClient(new InfluxDBClientOptions(this._optionsMonitor.CurrentValue.Url)
            {
                Token = this._optionsMonitor.CurrentValue.ApiKey,
                Org = this._optionsMonitor.CurrentValue.Org,
                Bucket = this._optionsMonitor.CurrentValue.Database,
            });
        }

        public async Task WriteBatteryData(DateTime time, decimal ampere, decimal ampereH, decimal percent, decimal voltage)
        {
            var point = PointData.Measurement("battery")
                .Timestamp(time, InfluxDB.Client.Api.Domain.WritePrecision.S)
                .Field("ampere", ampere)
                .Field("ampereH", ampereH)
                .Field("percent", percent)
                .Field("voltage", voltage);
            await this._client.GetWriteApiAsync().WritePointAsync(point, InfluxConstants.DATABASE);
        }
        public async Task WriteSolarData(DateTime time, decimal ampere, byte state)
        {
            var point = PointData.Measurement("solar")
                .Timestamp(time, InfluxDB.Client.Api.Domain.WritePrecision.S)
                .Field("ampere", ampere)
                .Field("state", state);
            await this._client.GetWriteApiAsync().WritePointAsync(point, InfluxConstants.DATABASE);
        }

        public async Task WriteSensorData(DateTime time, int id, decimal temp)
        {
            var point = PointData.Measurement("sensor")
                .Timestamp(time, InfluxDB.Client.Api.Domain.WritePrecision.S)
                .Tag("id", id.ToString())
                .Field("temperature", temp);
            try
            {
                await this._client.GetWriteApiAsync().WritePointAsync(point, InfluxConstants.DATABASE);
            }
            catch (Exception ex)
            {
            }
        }



        public void Dispose()
        {
            this._client.Dispose();
        }
    }
    public interface IInfluxContext : IDisposable
    {
        Task WriteBatteryData(DateTime time, decimal ampere, decimal ampereH, decimal percent, decimal voltage);
        Task WriteSensorData(DateTime time, int id, decimal temp);
        Task WriteSolarData(DateTime time, decimal ampere, byte state);
    }
}

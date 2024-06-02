using InfluxDB.Client;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.DataAccess
{
    public class InfluxContext : IDisposable
    {
        private readonly InfluxDBClient _client;

        public InfluxContext()
        {
            this._client = InfluxDBClientFactory.Create(new InfluxDBClientOptions(InfluxConstants.URL)
            {
                Token = InfluxConstants.API_KEY,
                Org = "chdCaraVan",
                Bucket = InfluxConstants.DATABASE,
            });
        }

        public async Task WriteBatteryData(DateTime time, decimal ampere, decimal ampereH, decimal percent)
        {
            var point = PointData.Measurement("battery")
                .Timestamp(time, InfluxDB.Client.Api.Domain.WritePrecision.S)
                .Field("ampere", ampere)
                .Field("ampereH", ampereH)
                .Field("percent", percent);
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

            await this._client.GetWriteApiAsync().WritePointAsync(point, InfluxConstants.DATABASE);
        }



        public void Dispose()
        {
            this._client.Dispose();
        }
    }
}

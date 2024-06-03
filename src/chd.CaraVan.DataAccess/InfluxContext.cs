using InfluxDB3.Client;
using InfluxDB3.Client.Write;
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
    private const string MES_BATTERY = "battery";
    private const string MES_SOLAR = "solar";
    private const string MES_SENSOR = "sensor";

    private const string FIELD_AMPERE = "ampere";
    private const string FIELD_AMPEREH = "ampereH";
    private const string FIELD_PERCENT = "percent";
    private const string FIELD_VOLTAGE = "voltage";
    private const string FIELD_TEMPERATURE = "temperature";
    private const string FIELD_STATE = "state";
    private const string FIELD_TIME = "time";

    private const string TAG_ID = "temperature";

    private readonly InfluxDBClient _client;
    private readonly IOptionsMonitor<InfluxSettings> _optionsMonitor;

    public InfluxContext(IOptionsMonitor<InfluxSettings> optionsMonitor)
    {
        this._client = new InfluxDBClient(optionsMonitor.CurrentValue.Url, optionsMonitor.CurrentValue.ApiKey);
        this._optionsMonitor = optionsMonitor;
    }

    public async Task WriteBatteryData(DateTime time, decimal ampere, decimal ampereH, decimal percent, decimal voltage, CancellationToken cancellationToken)
    {
        var point = PointData.Measurement(MES_BATTERY)
            .SetTimestamp(time)
            .SetField(FIELD_AMPERE, ampere)
            .SetField(FIELD_AMPEREH, ampereH)
            .SetField(FIELD_PERCENT, percent)
            .SetField(FIELD_VOLTAGE, voltage);
        await this._client.WritePointAsync(point, database: this._optionsMonitor.CurrentValue.Database, cancellationToken: cancellationToken);
    }
    public async Task WriteSolarData(DateTime time, decimal ampere, byte state, CancellationToken cancellationToken)
    {
        var point = PointData.Measurement(MES_SOLAR)
            .SetTimestamp(time)
            .SetField(FIELD_AMPERE, ampere)
            .SetField(FIELD_STATE, state);
        await this._client.WritePointAsync(point, database: this._optionsMonitor.CurrentValue.Database, cancellationToken: cancellationToken);
    }

    public async Task WriteSensorData(DateTime time, int id, decimal temp, CancellationToken cancellationToken)
    {
        var point = PointData.Measurement(MES_SENSOR)
            .SetTimestamp(time)
            .SetTag(TAG_ID, id.ToString())
            .SetField(FIELD_TEMPERATURE, temp);
        await this._client.WritePointAsync(point, database: this._optionsMonitor.CurrentValue.Database, cancellationToken: cancellationToken);
    }
    //public async Task GetSolarData(CancellationToken cancellationToken)
    //{
    //    var sql = @$"SELECT * FROM {MES_SOLAR}
    //                    WHERE time >= now() - interval '1 hour'";
    //    try
    //    {
    //        await foreach (var res in this._client.Query(sql, database: this._optionsMonitor.CurrentValue.Database))
    //        {
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //}


    public void Dispose()
    {
        this._client.Dispose();
    }
}
public interface IInfluxContext : IDisposable
{
    //Task GetSolarData(CancellationToken cancellationToken);
    Task WriteBatteryData(DateTime time, decimal ampere, decimal ampereH, decimal percent, decimal voltage, CancellationToken cancellationToken);
    Task WriteSensorData(DateTime time, int id, decimal temp, CancellationToken cancellationToken);
    Task WriteSolarData(DateTime time, decimal ampere, byte state, CancellationToken cancellationToken);
}
}

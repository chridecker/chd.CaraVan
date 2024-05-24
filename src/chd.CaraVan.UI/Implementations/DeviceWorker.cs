using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.DataAccess.Repositories;
using chd.CaraVan.Devices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace chd.CaraVan.UI.Implementations
{
    public class DeviceWorker : BackgroundService
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IOptionsMonitor<DeviceSettings> _optionsMonitor;
        private readonly IDeviceDataRepository _deviceDataRepository;
        private RuuviTag _tag;

        public DeviceWorker(ILogger<DeviceWorker> logger, IOptionsMonitor<DeviceSettings> optionsMonitor, IDeviceDataRepository deviceDataRepository)
        {
            this._logger = logger;
            this._optionsMonitor = optionsMonitor;
            this._deviceDataRepository = deviceDataRepository;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.StartDevices(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await this._tag?.DisconnectAsync();
            await base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this._deviceDataRepository.Clean(DateTime.Now.AddDays(-1));
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task StartDevices(CancellationToken cancellationToken)
        {
            this._tag = new RuuviTag(this._logger, this._optionsMonitor.CurrentValue.Devices.Select(s => new Devices.Contracts.Dtos.RuvviTag.RuuviTagConfiguration
            {
                BLEAdapter = "hci0",
                DeviceAddress = s.UID
            }));

            this._tag.DataReceived += this.Tag_DataReceived;
            await this._tag.ConnectAsync(cancellationToken);
        }

        private void Tag_DataReceived(object? sender, Devices.Contracts.Dtos.RuvviTag.RuuviTagEventArgs e)
        {
            var device = this._optionsMonitor.CurrentValue.Devices.FirstOrDefault(x => x.UID == e.UID);
            this._deviceDataRepository.Add(new Contracts.Dtos.DeviceData(0, e.DateTime, Contracts.Enums.EDataType.Temperature, e.Data.temperature)
            {
                DeviceId = device.Id,
            });
            this._deviceDataRepository.Add(new Contracts.Dtos.DeviceData(0, e.DateTime, Contracts.Enums.EDataType.Humidity, e.Data.humidity)
            {
                DeviceId = device.Id,
            });
        }
    }
}

using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.Devices;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Devices.Contracts.Dtos.Votronic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace chd.CaraVan.UI.Implementations
{
    public class DeviceWorker : BackgroundService
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IOptionsMonitor<DeviceSettings> _optionsMonitor;
        private readonly IDataService _dataService;
        private BLEManager _tag;

        public DeviceWorker(ILogger<DeviceWorker> logger, IOptionsMonitor<DeviceSettings> optionsMonitor, IDataService dataService)
        {
            this._logger = logger;
            this._optionsMonitor = optionsMonitor;
            this._dataService = dataService;
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
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task StartDevices(CancellationToken cancellationToken)
        {
            this._tag = new BLEManager(this._logger, this._optionsMonitor.CurrentValue.RuuviTags.Select(s => new RuuviTagConfiguration
            {
                Alias = s.Name,
                DeviceAddress = s.UID,
                Id = s.Id,
            }), new VotronicConfiguration
            {
                Id = this._optionsMonitor.CurrentValue.Votronic?.Id ?? 0,
                DeviceAddress = this._optionsMonitor.CurrentValue.Votronic?.UID,
                Alias = this._optionsMonitor.CurrentValue.Votronic?.Name 
            });

            this._tag.RuuviTagDataReceived += this.RuuviTag_DataReceived;
            await this._tag.ConnectAsync(cancellationToken);
        }

        private void RuuviTag_DataReceived(object? sender, RuuviTagEventArgs e)
        {
            var device = this._optionsMonitor.CurrentValue.RuuviTags.FirstOrDefault(x => x.Id == e.Id);

            this._dataService.AddData(device.Id, new DeviceData(e.DateTime, EDataType.Temperature, e.Data.Temperature ?? 0)
            {
                DeviceId = device.Id
            });
            this._dataService.AddData(device.Id, new DeviceData(e.DateTime, EDataType.Humidity, e.Data.Humidity ?? 0)
            {
                DeviceId = device.Id
            });
        }
    }
}

using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.Devices;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Devices.Contracts.Dtos.Votronic;
using chd.CaraVan.UI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Extensions;

namespace chd.CaraVan.UI.Implementations
{
    public class DeviceWorker : BackgroundService
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IHubContext<DataHub, IDataHub> _hub;
        private readonly IOptionsMonitor<DeviceSettings> _optionsMonitor;
        private readonly IRuuviTagDataService _dataService;
        private readonly IVotronicDataService _votronicDataService;
        private BLEManager _tag;

        public DeviceWorker(ILogger<DeviceWorker> logger,
             IHubContext<DataHub, IDataHub> hub,
            IOptionsMonitor<DeviceSettings> optionsMonitor, IRuuviTagDataService dataService, IVotronicDataService votronicDataService)
        {
            this._logger = logger;
            this._hub = hub;
            this._optionsMonitor = optionsMonitor;
            this._dataService = dataService;
            this._votronicDataService = votronicDataService;
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
                Alias = this._optionsMonitor.CurrentValue.Votronic?.Name,
                BatteryAH = this._optionsMonitor.CurrentValue.Votronic.BatteryAH
            });

            this._tag.RuuviTagDataReceived += this.RuuviTag_DataReceived;
            this._tag.VotronicDataReceived += this._tag_VotronicDataReceived;

            if (OperatingSystem.IsLinux())
            {
                await this._tag.ConnectAsync(cancellationToken);
            }
        }

        private void _tag_VotronicDataReceived(object? sender, VotronicEventArgs e)
        {
            if (e.BatteryData is not null)
            {
                var data = new Contracts.Dtos.VotronicBatteryData()
                {
                    DateTime = e.DateTime,
                    Ampere = e.BatteryData.Ampere,
                    AmpereH = e.BatteryData.LeftAH,
                    Voltage = e.BatteryData.Voltage,
                    Percent = e.BatteryData.Percent
                };
                this._votronicDataService.AddData(data);
                this._hub.Clients.All.VotronicData(data);
            }
            if (e.SolarData is not null)
            {
                var data = new Contracts.Dtos.VotronicSolarData()
                {
                    DateTime = e.DateTime,
                    Ampere = e.SolarData.Ampere,
                    WattH = e.SolarData.WattH,
                    AmpereH = e.SolarData.AH,
                    State = e.SolarData.State,
                    Voltage = e.SolarData.Voltage
                };
                this._votronicDataService.AddData(data);
                this._hub.Clients.All.VotronicData(data);
            }
        }

        private void RuuviTag_DataReceived(object? sender, RuuviTagEventArgs e)
        {
            var device = this._optionsMonitor.CurrentValue.RuuviTags.FirstOrDefault(x => x.Id == e.Id);
            var data = new RuuviTagDeviceData(e.DateTime, EDataType.Temperature, e.Data.Temperature ?? 0)
            {
                DeviceId = device.Id
            };
            this._dataService.AddData(device.Id, data);
            this._hub.Clients.All.RuuviTagData(data);

            data = new RuuviTagDeviceData(e.DateTime, EDataType.Humidity, e.Data.Humidity ?? 0)
            {
                DeviceId = device.Id
            };
            this._dataService.AddData(device.Id, data);
            this._hub.Clients.All.RuuviTagData(data);
        }
    }
}

using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.Devices;
using chd.CaraVan.Devices.Contracts.Dtos.Pi;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Devices.Contracts.Dtos.Victron;
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
        private readonly IOptionsMonitor<AesSettings> _optionsMonitorAes;
        private readonly IOptionsMonitor<PiSettings> _optionsMonitorPi;
        private readonly IOptionsMonitor<DeviceSettings> _optionsMonitor;
        private readonly IRuuviTagDataService _dataService;
        private readonly IVotronicDataService _votronicDataService;
        private BLEManager _tag;
        private IPiManager _pi;
        private readonly IAESManager _aesManager;
        private readonly IVictronDataService _victronDataService;

        public DeviceWorker(ILogger<DeviceWorker> logger,
             IHubContext<DataHub, IDataHub> hub, IOptionsMonitor<AesSettings> optionsMonitorAes,
             IOptionsMonitor<PiSettings> optionsMonitorPi,
             IPiManager piManager, IAESManager aesManager, IVictronDataService victronDataService,
            IOptionsMonitor<DeviceSettings> optionsMonitor, IRuuviTagDataService dataService, IVotronicDataService votronicDataService)
        {
            this._logger = logger;
            this._hub = hub;
            this._optionsMonitorAes = optionsMonitorAes;
            this._optionsMonitorPi = optionsMonitorPi;
            this._pi = piManager;
            this._aesManager = aesManager;
            this._victronDataService = victronDataService;
            this._optionsMonitor = optionsMonitor;
            this._dataService = dataService;
            this._votronicDataService = votronicDataService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            this.StartPi();
            await this.StartDevices(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this._pi.Stop();
            this._aesManager.StateSwitched += this._aesManager_StateSwitched;
            await this._tag?.DisconnectAsync();
            await base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await this._aesManager.CheckForActive();
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        private void StartPi()
        {
            this._aesManager.StateSwitched += this._aesManager_StateSwitched;
            if (OperatingSystem.IsLinux())
            {
                this._pi.Start();
            }
        }

        private async void _aesManager_StateSwitched(object? sender, bool e)
        {
            foreach (var pin in this._optionsMonitorPi.CurrentValue.Gpios.Where(x => x.Type == Devices.Contracts.Enums.GpioType.Aes))
            {
                await this._pi.Write(pin.Pin, this._optionsMonitorAes.CurrentValue.IsActive && e);
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
            },
            new Devices.Contracts.Dtos.Victron.VictronConfiguration
            {
                Id = this._optionsMonitor.CurrentValue.Victron.Id,
                DeviceAddress = this._optionsMonitor.CurrentValue.Victron.UID,
                Alias = this._optionsMonitor.CurrentValue.Victron.Name,
                Aes = this._optionsMonitor.CurrentValue.Victron.Aes
            });

            this._tag.RuuviTagDataReceived += this.RuuviTag_DataReceived;
            this._tag.VotronicDataReceived += this._tag_VotronicDataReceived;
            this._tag.VictronDataReceived += this._tag_VictronDataReceived;

            if (OperatingSystem.IsLinux())
            {
                await this._tag.ConnectAsync(cancellationToken);
            }
        }

        private async void _tag_VictronDataReceived(object? sender, VictronEventArgs e)
        {
            this._victronDataService.Add(new Contracts.Dtos.VictronData()
            {
                AmpereAC = e.Data.AmpereAC,
                AmpereDC = e.Data.Ampere,
                Error = e.Data.Error,
                State = e.Data.State,
                DateTime = e.DateTime
            });
            await this._hub.Clients.All.VictronData();
        }

        private async void _tag_VotronicDataReceived(object? sender, VotronicEventArgs e)
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
                await this._hub.Clients.All.VotronicData();
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
                    Voltage = e.SolarData.Voltage,
                    VoltageSolar = e.SolarData.VoltageSolar,
                    LoadingPhase = e.SolarData.LoadingPhase
                };
                this._votronicDataService.AddData(data);
                await this._hub.Clients.All.VotronicData();
            }
        }

        private async void RuuviTag_DataReceived(object? sender, RuuviTagEventArgs e)
        {
            var device = this._optionsMonitor.CurrentValue.RuuviTags.FirstOrDefault(x => x.Id == e.Id);
            var data = new RuuviTagDeviceData(e.DateTime, EDataType.Temperature, e.Data.Temperature ?? 0)
            {
                DeviceId = device.Id
            };
            this._dataService.AddData(device.Id, data);
            await this._hub.Clients.All.RuuviTagData();
        }
    }
}

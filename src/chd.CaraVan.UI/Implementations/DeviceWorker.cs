using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.DataAccess.Repositories;
using chd.CaraVan.Devices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class DeviceWorker : BackgroundService
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IOptionsMonitor<DeviceSettings> _optionsMonitor;
        private readonly IDeviceDataRepository _deviceDataRepository;
        private List<RuuviTag> _tags;

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
            this._tags = new List<RuuviTag>();
            foreach (var device in this._optionsMonitor.CurrentValue.Devices)
            {
                var tag = new RuuviTag(this._logger, new Devices.Contracts.Dtos.RuvviTag.RuuviTagConfiguration
                {
                    BLEAdapter = "hci0",
                    DeviceAddress = device.UID
                });
                tag.DataReceived += this.Tag_DataReceived;
                await tag.ConnectAsync(cancellationToken);
                this._tags.Add(tag);
            }
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

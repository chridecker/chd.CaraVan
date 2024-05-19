using chd.CaraVan.Devices.Contracts.Devices.Interfaces;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices
{
    public class RuuviTag : IRuuviITag
    {
        private Adapter _adapter;
        private Device _device;
        private readonly RuvviTagConfiguration _config;
        private readonly ILogger _logger;

        public RuuviTag(ILogger logger, RuvviTagConfiguration config)
        {
            this._config = config;
            this._logger = logger;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            var adapters = await BlueZManager.GetAdaptersAsync();

            this._adapter = adapters.FirstOrDefault();
            this._logger?.LogInformation($"Choose Adapter: {this._adapter?.Name}");

            //this._device = await this._adapter.GetDeviceAsync(this._config.DeviceAddress);

            //var prop = await this._device.GetPropertiesAsync();
            //this._logger?.LogInformation($"Device. {prop.Name} - {prop.Address}, {prop.Connected} / {prop.IsConnected}");

            //var data = prop.ServiceData;

            //foreach (var d in data)
            //{
            //    this._logger?.LogInformation($"Service {d.Key} -> {d.Value}");
            //}

            this._adapter.DeviceFound += this._adapter_DeviceFound;

            await this._adapter.StartDiscoveryAsync();
        }

        private async Task _adapter_DeviceFound(Adapter sender, DeviceFoundEventArgs eventArgs)
        {
            var device = eventArgs.Device;

            var prop = await device.GetPropertiesAsync();

            if (prop.Address == this._config.DeviceAddress)
            {
                this._logger?.LogInformation($"Device {prop.Address}, {prop.Connected} / {prop.IsConnected}");
                var data = prop.ServiceData;
                foreach (var d in data)
                {
                    this._logger?.LogInformation($"Service {d.Key} -> {d.Value}");
                }
            }
        }

        private async Task Device_ServicesResolved(Device sender, BlueZEventArgs eventArgs)
        {

        }

        private async Task Device_Connected(Device sender, BlueZEventArgs eventArgs)
        {

        }


        public async Task<RuuviTagData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            await this._device?.DisconnectAsync();
            await this._adapter.StopDiscoveryAsync();
        }
    }
}

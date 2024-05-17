using chd.CaraVan.Devices.Contracts.Devices.Interfaces;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
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

        public RuuviTag(RuvviTagConfiguration config)
        {
            this._config = _config;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {


            await BlueZManager.GetAdaptersAsync();

            this._adapter = await BlueZManager.GetAdapterAsync(this._config.BLEAdapter);

            var devices = await this._adapter.GetDevicesAsync();

            _device = await this._adapter.GetDeviceAsync(this._config.DeviceAddress);
            _device.Connected += this.Device_Connected;
            _device.Disconnected += this.Device_Connected;
            _device.ServicesResolved += this.Device_ServicesResolved;
            await _device.ConnectAsync();


            //this._adapter.DeviceFound += this.Adapter1_DeviceFound;
            //await this._adapter.StartDiscoveryAsync();
        }

       

        private async Task Device_ServicesResolved(Device sender, BlueZEventArgs eventArgs)
        {

        }

        private async Task Device_Connected(Device sender, BlueZEventArgs eventArgs)
        {

        }

        private async Task Adapter1_DeviceFound(Adapter sender, DeviceFoundEventArgs eventArgs)
        {
            await this._adapter.StopDiscoveryAsync();
        }

        public Task<RuuviTagData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            await this._device.DisconnectAsync();
            await this._adapter.StopDiscoveryAsync();
        }
    }
}

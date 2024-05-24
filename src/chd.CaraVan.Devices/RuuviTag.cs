using chd.CaraVan.Devices.Contracts.Devices.Interfaces;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

namespace chd.CaraVan.Devices
{
    public class RuuviTag : IRuuviTag
    {
        private Adapter _adapter;
        private List<Device> _devices;
        private readonly IEnumerable<RuuviTagConfiguration> _config;
        private readonly ILogger _logger;

        public event EventHandler<RuuviTagEventArgs> DataReceived;


        public RuuviTag(ILogger logger, IEnumerable<RuuviTagConfiguration> config)
        {
            this._config = config;
            this._logger = logger;

            this._devices = new();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var adapters = await BlueZManager.GetAdaptersAsync();
                this._adapter = adapters.FirstOrDefault();
                this._logger?.LogDebug($"Choose Adapter: {this._adapter?.Name}");
                this._adapter.DeviceFound += this._adapter_DeviceFound;

                await this._adapter.StartDiscoveryAsync();
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
            }

        }

        private async Task _adapter_DeviceFound(Adapter sender, DeviceFoundEventArgs eventArgs)
        {
            var device = eventArgs.Device;

            var prop = await device.GetPropertiesAsync();
            if (this._config.Any(a => a.DeviceAddress == prop.Address))
            {
                var config = this._config.FirstOrDefault(x => x.DeviceAddress == prop.Address);
                this._logger?.LogInformation($"Found Device {config.Alias} [{prop.Address}]");
                await this.HandleDevice(device);
            }
        }

        private async Task HandleDevice(Device device)
        {
            device.ServicesResolved += this.Device_ServicesResolved1;
            await device.ConnectAsync();
            this._devices.Add(device);
            var all = await device.GetAllAsync();
            this._logger?.LogInformation($"{all.Name}, {all.Connected}, {all.ServicesResolved}");
        }

        private async Task Device_ServicesResolved1(Device device, BlueZEventArgs eventArgs)
        {
            string nordicUart = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
            string rxCharacterisitc = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
            string txCharacterisitc = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
            var service = await device.GetServiceAsync(nordicUart);
            if (service is not null)
            {
                this._logger?.LogDebug($"Found NordicUartService on device");
                var rxC = await service.GetCharacteristicAsync(rxCharacterisitc);
                var txC = await service.GetCharacteristicAsync(txCharacterisitc);
                try
                {
                    await rxC.StartNotifyAsync();
                    await txC.StartNotifyAsync();

                    rxC.Value += this.RxC_Value;

                }
                catch (Exception ex)
                {
                    this._logger?.LogError(ex, ex.Message);
                }
            }
            else
            {
                await this.ScanForServices(device);
            }
        }

        private async Task RxC_Value(GattCharacteristic characteristic, GattCharacteristicValueEventArgs e)
        {
            var service = await characteristic.GetServiceAsync();
            var device = await service.GetDeviceAsync();
            var address = await device.GetAddressAsync();
            var config = this._config.FirstOrDefault(x => x.DeviceAddress == address);

            this._logger?.LogInformation($"Received Value {config?.Alias}: {string.Join("-", e.Value)}");
        }

        private async Task ScanForServices(Device device)
        {

            foreach (var service in await device.GetServicesAsync())
            {
                var props = await service.GetAllAsync();
                this._logger?.LogDebug($"Service: {props.UUID}");
                foreach (var c in await service.GetCharacteristicsAsync())
                {
                    var cprops = await c.GetAllAsync();
                    this._logger?.LogDebug($"{cprops.UUID}, {cprops.Notifying}, {string.Join(",", cprops.Flags)}");
                    if (cprops.Notifying)
                    {
                        var characteristicUUID = await c.GetUUIDAsync();
                        var gatt = await service.GetCharacteristicAsync(characteristicUUID);
                        gatt.Value += this.Gatt_Value;
                    }

                    var val = await c.GetValueAsync();
                    var data = Encoding.UTF8.GetString(val);
                    this._logger?.LogDebug($"val: {data}");
                }
            }
        }

        private async Task Gatt_Value(GattCharacteristic sender, GattCharacteristicValueEventArgs e)
        {
            try
            {
                this._logger?.LogWarning($"Characteristic value (hex): {BitConverter.ToString(e.Value)}");

                this._logger?.LogWarning($"Characteristic value (UTF-8): \"{Encoding.UTF8.GetString(e.Value)}\"");
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
            }
        }

        private async Task Device_ServicesResolved(Device sender, BlueZEventArgs eventArgs)
        {

        }

        private async Task Device_Connected(Device sender, BlueZEventArgs eventArgs)
        {

        }

        private void InvokeDataReceived(RuuviTagData data) => this.DataReceived.Invoke(this, new RuuviTagEventArgs
        {
            Data = data,
            DateTime = DateTime.Now,
            // UID = this._config.DeviceAddress
        });


        public async Task<RuuviTagData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            foreach (var device in this._devices)
            {
                await device?.DisconnectAsync();
            }
            await this._adapter.StopDiscoveryAsync();
        }


    }
}

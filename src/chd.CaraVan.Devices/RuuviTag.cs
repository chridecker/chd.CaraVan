using chd.CaraVan.Devices.Contracts.Devices.Interfaces;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Microsoft.Extensions.Logging;
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
                this._logger?.LogInformation($"Defined Device {prop.Address}");
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

        private async Task Device_ServicesResolved1(Device sender, BlueZEventArgs eventArgs)
        {
            string serviceUUID = "0000180a-0000-1000-8000-00805f9b34fb";
            string characteristicUUID = "00002a24-0000-1000-8000-00805f9b34fb";

            foreach (var service in await sender.GetServicesAsync())
            {
                var props = await service.GetAllAsync();
                this._logger?.LogInformation($"Service: {props.UUID}");
                foreach (var c in await service.GetCharacteristicsAsync())
                {
                    var cprops = await c.GetAllAsync();
                    this._logger?.LogInformation($"{cprops.UUID}, {cprops.Notifying}, {string.Join(",", cprops.Flags)}");
                    if (cprops.Notifying)
                    {
                        characteristicUUID = await c.GetUUIDAsync();
                        var gatt = await service.GetCharacteristicAsync(characteristicUUID);
                        gatt.Value += this.Gatt_Value;
                    }

                    var val = await c.GetValueAsync();
                    var data = Encoding.UTF8.GetString(val);
                    this._logger?.LogInformation($"val: {data}");
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

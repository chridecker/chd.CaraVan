using chd.CaraVan.Devices.Contracts.Devices.Interfaces;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;
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

                this._adapter.DeviceFound += this._adapter_DeviceFound;

                await this._adapter.StartDiscoveryAsync();
                this._logger?.LogDebug($"Choose Adapter: {this._adapter?.Name}");
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
            }
        }

        private async Task _adapter_DeviceFound(Adapter sender, DeviceFoundEventArgs e)
        {
            var device = e.Device;
            var uid = await device.GetAddressAsync();
            if (this._config.Any(a => a.DeviceAddress.ToLower() == uid.ToLower()))
            {
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
            string txCharacterisitc = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
            var service = await device.GetServiceAsync(nordicUart);
            if (service is not null)
            {
                this._logger?.LogDebug($"Found NordicUartService on device");
                var txC = await service.GetCharacteristicAsync(txCharacterisitc);

                try
                {
                    txC.Value += RxC_Value;
                    var txCProp = await txC.GetAllAsync();
                    this._logger?.LogInformation($"Start TX Notifify, {txCProp.Notifying}");
                }
                catch (Exception ex)
                {
                    this._logger?.LogError(ex, ex.Message);
                }
            }
        }

        private async Task RxC_Value(GattCharacteristic characteristic, GattCharacteristicValueEventArgs e)
        {
            var service = await characteristic.GetServiceAsync();
            var device = await service.GetDeviceAsync();
            var address = await device.GetAddressAsync();
            var config = this._config.FirstOrDefault(x => x.DeviceAddress == address);
            this._logger?.LogInformation($"Received Value {config?.Alias}: {string.Join("-", e.Value)}");
            //this._logger?.LogInformation(Convert.ToHexString(e.Value));
            this.InvokeDataReceived(new RuuviTagData(e.Value), config);
        }

        private void InvokeDataReceived(RuuviTagData data, RuuviTagConfiguration config) => this.DataReceived.Invoke(this, new RuuviTagEventArgs
        {
            Data = data,
            DateTime = DateTime.Now,
            UID = config.DeviceAddress
        });


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

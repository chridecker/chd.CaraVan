using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Devices.Contracts.Dtos.Votronic;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace chd.CaraVan.Devices
{
    public class BLEManager
    {
        private Adapter _adapter;
        private List<Device> _devices;
        private readonly IEnumerable<RuuviTagConfiguration> _ruuviTagConfig;
        private readonly VotronicConfiguration _votronicConfiguration;
        private readonly ILogger _logger;

        public event EventHandler<RuuviTagEventArgs> RuuviTagDataReceived;


        public BLEManager(ILogger logger, IEnumerable<RuuviTagConfiguration> config, VotronicConfiguration votronicConfiguration)
        {
            this._ruuviTagConfig = config;
            this._votronicConfiguration = votronicConfiguration;
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
            if (this._ruuviTagConfig.Any(a => a.DeviceAddress.ToLower() == uid.ToLower())
                || uid.ToLower() == this._votronicConfiguration.DeviceAddress.ToLower())
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
            var address = await device.GetAddressAsync();
            if (this._ruuviTagConfig.Any(a => a.DeviceAddress.ToLower() == address.ToLower()))
            {
                string nordicUart = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
                string txCharacterisitc = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
                var service = await device.GetServiceAsync(nordicUart);
                if (service is not null)
                {
                    var txC = await service.GetCharacteristicAsync(txCharacterisitc);
                    txC.Value += RuuviTag_Value;
                    var txCProp = await txC.GetAllAsync();
                }
            }
            else  if(address.ToLower() == this._votronicConfiguration.DeviceAddress.ToLower())
            {

            }
        }

        private async Task RuuviTag_Value(GattCharacteristic characteristic, GattCharacteristicValueEventArgs e)
        {
            var service = await characteristic.GetServiceAsync();
            var device = await service.GetDeviceAsync();
            var address = await device.GetAddressAsync();
            var config = this._ruuviTagConfig.FirstOrDefault(x => x.DeviceAddress == address);
            this._logger?.LogTrace($"Received Value {config?.Alias}: {string.Join("-", e.Value)}");
            this.InvokeDataReceived(new RuuviTagData(e.Value), config);
        }

        private void InvokeDataReceived(RuuviTagData data, RuuviTagConfiguration config) => this.RuuviTagDataReceived.Invoke(this, new RuuviTagEventArgs
        {
            Data = data,
            DateTime = DateTime.Now,
            Id = config.Id
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

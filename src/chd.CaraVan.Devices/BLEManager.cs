using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Devices.Contracts.Dtos.Victron;
using chd.CaraVan.Devices.Contracts.Dtos.Votronic;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Tmds.DBus;

namespace chd.CaraVan.Devices
{
    public class BLEManager
    {

        private const string NORDIC_UART_SVC = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
        private const string TX_CHARACTERISTIC = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";

        private const string VOTRONIC_SVC = "d0cb6aa7-8548-46d0-99f8-2d02611e5270";
        private const string BATTERY_CHARACTERISTIC = "9a082a4e-5bcc-4b1d-9958-a97cfccfa5ec";
        private const string SOLAR_CHARACTERISTIC = "971ccec2-521d-42fd-b570-cf46fe5ceb65";

        private const string VICTRONENERGY_SVC = "";
        private const string VICTRONENERGY_AC_CHARACTERISTIC = "";

        private Adapter _adapter;
        private IDictionary<string, Device> _devices;
        private readonly IEnumerable<RuuviTagConfiguration> _ruuviTagConfig;
        private readonly VotronicConfiguration _votronicConfiguration;
        private readonly VictronConfiguration _victronConfiguration;
        private readonly ILogger _logger;

        public event EventHandler<RuuviTagEventArgs> RuuviTagDataReceived;
        public event EventHandler<VotronicEventArgs> VotronicDataReceived;
        public event EventHandler<VictronEventArgs> VictronDataReceived;


        public BLEManager(ILogger logger, IEnumerable<RuuviTagConfiguration> config, VotronicConfiguration votronicConfiguration, VictronConfiguration victronConfiguration)
        {
            this._ruuviTagConfig = config;
            this._votronicConfiguration = votronicConfiguration;
            this._victronConfiguration = victronConfiguration;
            this._logger = logger;
            this._devices = new Dictionary<string, Device>();
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
                || uid.ToLower() == this._votronicConfiguration.DeviceAddress.ToLower()
                || uid.ToLower() == this._victronConfiguration.DeviceAddress.ToLower())
            {
                await this.HandleDevice(device);
                if (this._devices.Count == this._ruuviTagConfig.Count() + 1)
                {
                    await this._adapter.StopDiscoveryAsync();
                }

            }
        }

        private async Task HandleDevice(Device device)
        {
            device.Connected += this.Device_Connected;
            device.Disconnected += this.Device_Disconnected;
            device.ServicesResolved += this.Device_ServicesResolved;
            var all = await device.GetAllAsync();

            await device.ConnectAsync();
            this._devices[all.Address] = device;

            this._logger?.LogInformation($"{all.Name}, {all.Connected}, {all.ServicesResolved}");
        }

        private async Task Device_Disconnected(Device device, BlueZEventArgs eventArgs)
        {
            var address = await device.GetAddressAsync();
            if (this._devices.ContainsKey(address))
            {
                this._devices.Remove(address);
                await this._adapter.StartDiscoveryAsync();
            }
        }

        private async Task Device_Connected(Device device, BlueZEventArgs eventArgs)
        {
            var address = await device.GetAddressAsync();
            var paired = await device.GetPairedAsync();
            if ((address.ToLower() == this._votronicConfiguration.DeviceAddress.ToLower()
                || address.ToLower() == this._victronConfiguration.DeviceAddress.ToLower())
                && !paired)
            {
                this._logger?.LogError($"Not paired");
            }
        }

        private async Task Device_ServicesResolved(Device device, BlueZEventArgs eventArgs)
        {
            var address = await device.GetAddressAsync();
            if (this._ruuviTagConfig.Any(a => a.DeviceAddress.ToLower() == address.ToLower()))
            {
                var service = await device.GetServiceAsync(NORDIC_UART_SVC);
                if (service is not null)
                {
                    var txC = await service.GetCharacteristicAsync(TX_CHARACTERISTIC);
                    txC.Value += RuuviTag_Value;
                }
            }
            else if (address.ToLower() == this._votronicConfiguration.DeviceAddress.ToLower())
            {
                var service = await device.GetServiceAsync(VOTRONIC_SVC);
                if (service is not null)
                {
                    var batteryC = await service.GetCharacteristicAsync(BATTERY_CHARACTERISTIC);
                    batteryC.Value += VotronicBattery_Received;

                    var solarC = await service.GetCharacteristicAsync(SOLAR_CHARACTERISTIC);
                    solarC.Value += VotronicSolar_Received;
                }
            }
            else if (address.ToLower() == this._victronConfiguration.DeviceAddress.ToLower())
            {
                var service = await device.GetServiceAsync(VICTRONENERGY_SVC);
                if (service is not null)
                {
                    var dataAd = await service.GetCharacteristicAsync(VICTRONENERGY_AC_CHARACTERISTIC);
                    dataAd.Value += VictronAC_Data;
                }
            }
        }

        private async Task RuuviTag_Value(GattCharacteristic characteristic, GattCharacteristicValueEventArgs e)
        {
            var service = await characteristic.GetServiceAsync();
            var device = await service.GetDeviceAsync();
            var address = await device.GetAddressAsync();
            var config = this._ruuviTagConfig.FirstOrDefault(x => x.DeviceAddress == address);
            this._logger?.LogTrace($"Received Value {config?.Alias}: {string.Join("-", e.Value)}");
            this.InvokeRuuviTagDataReceived(new RuuviTagData(e.Value), config);
        }

        private async Task VotronicBattery_Received(GattCharacteristic characteristic, GattCharacteristicValueEventArgs e)
        {
            var service = await characteristic.GetServiceAsync();
            var device = await service.GetDeviceAsync();
            var address = await device.GetAddressAsync();
            var config = this._ruuviTagConfig.FirstOrDefault(x => x.DeviceAddress == address);
            this._logger?.LogTrace($"Received Battery Value {config?.Alias}: {string.Join("-", e.Value)}");
            this.InvokeVotronicDataReceived(new VotronicBatteryData(e.Value, this._votronicConfiguration.BatteryAH));
        }

        private async Task VotronicSolar_Received(GattCharacteristic characteristic, GattCharacteristicValueEventArgs e)
        {
            this._logger?.LogTrace($"Received Solar Value {this._votronicConfiguration?.Alias}: {string.Join("-", e.Value)}");
            this.InvokeVotronicDataReceived(new VotronicSolarData(e.Value));
        }

        private async Task VictronAC_Data(GattCharacteristic characteristic, GattCharacteristicValueEventArgs e)
        {
            this._logger?.LogDebug($"Received Victron AC Value {this._victronConfiguration?.Alias}: {string.Join("-", e.Value)}");
            this.InvokeVictronDataReceived(new VictronData(e.Value));
        }

        private void InvokeRuuviTagDataReceived(RuuviTagData data, RuuviTagConfiguration config) => this.RuuviTagDataReceived.Invoke(this, new RuuviTagEventArgs
        {
            Data = data,
            DateTime = DateTime.Now,
            Id = config.Id
        });

        private void InvokeVotronicDataReceived(VotronicData data) => this.VotronicDataReceived.Invoke(this, new VotronicEventArgs
        {
            DateTime = DateTime.Now,
            BatteryData = data is VotronicBatteryData b ? b : null,
            SolarData = data is VotronicSolarData s ? s : null
        });

        private void InvokeVictronDataReceived(VictronData data) => this.VictronDataReceived.Invoke(this, new VictronEventArgs
        {
            DateTime = DateTime.Now,
            Data = data
        });


        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            foreach (var device in this._devices)
            {
                await device.Value?.DisconnectAsync();
            }
            if (await this._adapter.GetDiscoveringAsync())
            {
                await this._adapter.StopDiscoveryAsync();
            }
        }


    }
}

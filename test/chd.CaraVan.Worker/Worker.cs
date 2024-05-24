using chd.CaraVan.Devices;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using Linux.Bluetooth;
using Microsoft.Extensions.Options;

namespace chd.CaraVan.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<RuuviTagConfiguration> _optionsMonitor;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<RuuviTagConfiguration> optionsMonitor)
        {
            _logger = logger;
            this._optionsMonitor = optionsMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var adapter = (await BlueZManager.GetAdaptersAsync()).First();
            this._logger?.LogInformation($"Choose Adapter: {adapter?.Name}");

            //this._device = await this._adapter.GetDeviceAsync(this._config.DeviceAddress);

            //var prop = await this._device.GetPropertiesAsync();
            //this._logger?.LogInformation($"Device. {prop.Name} - {prop.Address}, {prop.Connected} / {prop.IsConnected}");

            //var data = prop.ServiceData;

            //foreach (var d in data)
            //{
            //    this._logger?.LogInformation($"Service {d.Key} -> {d.Value}");
            //}

            adapter.DeviceFound += this._adapter_DeviceFound;

            await adapter.StartDiscoveryAsync();


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        private async Task _adapter_DeviceFound(Adapter sender, DeviceFoundEventArgs eventArgs)
        {
            var device = eventArgs.Device;
            var uid = await device.GetUUIDsAsync();
            this._logger?.LogInformation($"Found Device: {string.Join(":", uid)}");
        }
    }
}

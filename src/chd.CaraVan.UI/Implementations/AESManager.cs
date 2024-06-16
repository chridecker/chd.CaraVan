using chd.CaraVan.Contracts.Settings;
using Microsoft.Extensions.Options;

namespace chd.CaraVan.UI.Implementations
{
    public class AESManager : IAESManager
    {

        private DateTime? _solarAesOffSince;
        private bool _isActive = false;
        private readonly IOptionsMonitor<AesSettings> _optionsMonitor;
        private readonly IVotronicDataService _votronicDataService;

        public event EventHandler<bool> StateSwitched;


        public Task<bool> IsActive => Task.FromResult(this._isActive);
        public Task<DateTime?> SolarAesOffSince => Task.FromResult(this._solarAesOffSince);

        public Task<decimal?> BatteryLimit => Task.FromResult(this._optionsMonitor.CurrentValue.BatteryLimit);
        public Task<decimal?> SolarAmpLimit => Task.FromResult(this._optionsMonitor.CurrentValue.SolarAmpLimit);
        public Task<TimeSpan?> AesTimeout => Task.FromResult(this._optionsMonitor.CurrentValue.AesTimeout);

        public AESManager(IOptionsMonitor<AesSettings> optionsMonitor, IVotronicDataService votronicDataService)
        {
            this._optionsMonitor = optionsMonitor;
            this._votronicDataService = votronicDataService;
        }



        public async Task CheckForActive()
        {
            var solarAmp = (await this._votronicDataService.GetSolarData())?.Ampere ?? 0;
            var solarAES = (await this._votronicDataService.GetSolarData())?.AES ?? false;
            var batteryPercent = (await this._votronicDataService.GetBatteryData())?.Percent ?? 0;
            if (this._isActive)
            {
                if (solarAES && this._solarAesOffSince.HasValue) { this._solarAesOffSince = null; }
                if (!solarAES && !this._solarAesOffSince.HasValue) { this._solarAesOffSince = DateTime.Now; }
                if (this._optionsMonitor.CurrentValue.BatteryLimit.HasValue && batteryPercent < this._optionsMonitor.CurrentValue.BatteryLimit.Value)
                {
                    await this.Off();
                }
                if (!solarAES && (!this._optionsMonitor.CurrentValue.AesTimeout.HasValue
                        || (this._optionsMonitor.CurrentValue.AesTimeout.HasValue && this._solarAesOffSince.HasValue && this._solarAesOffSince.Value.Add(this._optionsMonitor.CurrentValue.AesTimeout.Value) < DateTime.Now)))
                {
                    await this.Off();
                }
                if (!solarAES && (!this._optionsMonitor.CurrentValue.SolarAmpLimit.HasValue
                    || (this._optionsMonitor.CurrentValue.SolarAmpLimit.HasValue && this._optionsMonitor.CurrentValue.SolarAmpLimit.Value > solarAmp)))
                {
                    await this.Off();
                }
            }
            else if (!this._isActive && solarAES)
            {
                if (this._optionsMonitor.CurrentValue.BatteryLimit.HasValue && batteryPercent > this._optionsMonitor.CurrentValue.BatteryLimit.Value)
                {
                    await this.SetActive();
                }
            }
        }

        public Task Off() => Task.Run(() =>
        {
            if (this._isActive)
            {
                this._isActive = false;
                this._solarAesOffSince = null;
                this.StateSwitched?.Invoke(this, this._isActive);
            }
        });

        public Task SetActive() => Task.Run(() =>
        {
            if (!this._isActive)
            {
                this._isActive = true;
                this.StateSwitched?.Invoke(IsActive, this._isActive);
            }
        });
    }
    public interface IAESManager
    {
        Task Off();
        Task CheckForActive();
        Task SetActive();
        Task<bool> IsActive { get; }
        Task<DateTime?> SolarAesOffSince { get; }
        Task<decimal?> BatteryLimit { get; }
        Task<decimal?> SolarAmpLimit { get; }
        Task<TimeSpan?> AesTimeout { get; }

        event EventHandler<bool> StateSwitched;
    }
}

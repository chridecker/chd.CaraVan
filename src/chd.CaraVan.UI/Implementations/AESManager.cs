using Microsoft.Extensions.Options;

namespace BlazorApp3
{
    public class AESManager : IAESManager
    {

        private DateTime? _isActiveSince;
        private bool _isActive;
        private readonly IOptionsMonitor<AesSettings> _optionsMonitor;
        private readonly IVotronicDataService _votronicDataService;


        public bool IsActive => this._isActive;
        public event EventHandler<bool> StateSwitched;

        public AESManager(IOptionsMonitor<AesSettings> optionsMonitor, IVotronicDataService votronicDataService)
        {
            this._optionsMonitor = optionsMonitor;
            this._votronicDataService = votronicDataService;
        }
        public void CheckForActive()
        {
            var solarAmp = this._votronicDataService.GetSolarData()?.Ampere ?? 0;
            var solarAES = this._votronicDataService.GetSolarData()?.AES ?? false;
            var batteryPercent = this._votronicDataService.GetBatteryData()?.Percent ?? 0;
            if (this._isActive)
            {
                if (this._optionsMonitor.CurrentValue.BatteryLimit.HasValue && batteryPercent < this._optionsMonitor.CurrentValue.BatteryLimit.Value)
                {
                    this.Off();
                }
                if (!solarAES && (!this._optionsMonitor.CurrentValue.AesTimeout.HasValue
                        || (this._optionsMonitor.CurrentValue.AesTimeout.HasValue && this._isActiveSince.HasValue && this._isActiveSince.Value.Add(this._optionsMonitor.CurrentValue.AesTimeout.Value) < DateTime.Now)))
                {
                    this.Off();
                }
                if (!solarAES && (!this._optionsMonitor.CurrentValue.SolarAmpLimit.HasValue
                    || (this._optionsMonitor.CurrentValue.SolarAmpLimit.HasValue && this._optionsMonitor.CurrentValue.SolarAmpLimit.Value > solarAmp)))
                {
                    this.Off();
                }
            }
            else if (!this._isActive && solarAES)
            {
                if (this._optionsMonitor.CurrentValue.BatteryLimit.HasValue && batteryPercent > this._optionsMonitor.CurrentValue.BatteryLimit.Value)
                {
                    this.SetActive();
                }
            }
        }

        public void Off()
        {
            if (this._isActive)
            {
                this._isActive = false;
                this._isActiveSince = null;
                this.StateSwitched?.Invoke(this, this._isActive);
            }
        }

        public void SetActive()
        {
            if (!this._isActive)
            {
                this._isActiveSince = DateTime.Now;
                this._isActive = true;
                this.StateSwitched?.Invoke(IsActive, this._isActive);
            }
        }
    }
    public interface IAESManager
    {
        void Off();
        void CheckForActive();
        void SetActive();
        bool IsActive { get; }

        event EventHandler<bool> StateSwitched;
    }
}

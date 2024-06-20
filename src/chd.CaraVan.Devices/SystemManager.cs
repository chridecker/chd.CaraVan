using chd.CaraVan.Devices.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace chd.CaraVan.Devices
{
    public class SystemManager : ISystemManager
    {
        private readonly ILogger<SystemManager> _logger;
        private readonly IEmailService _emailService;

        public SystemManager(ILogger<SystemManager> logger, IEmailService emailService)
        {
            this._logger = logger;
            this._emailService = emailService;
        }

        public void ChangeStateInTime(string service, TimeSpan span, CancellationToken cancellationToken) => _ = StopAfterTime(service, span, cancellationToken);

        private Task StopAfterTime(string service, TimeSpan span, CancellationToken cancellationToken) => Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(span);
            if (await timer.WaitForNextTickAsync(cancellationToken))
            {
                if ((await this.IsServiceRunning(service)).HasValue)
                {
                    await this._emailService.SendEmail($"Service '{service}' stopped @{DateTime.Now.ToString("dd.MM.yy HH:MM:ss")}", "Gestarted", cancellationToken);
                    await this.StopService(service);
                }
                else
                {

                    await this._emailService.SendEmail($"Service '{service}' started @{DateTime.Now.ToString("dd.MM.yy HH:MM:ss")}", "Gestarted", cancellationToken);
                    await this.StartService(service);
                }
            }
        }, cancellationToken);

        public async Task<bool> StartService(string service, CancellationToken cancellationToken = default)
        {
            if (!(await this.IsServiceRunning(service, cancellationToken)).HasValue)
            {
                await CommandService(service, "start", cancellationToken);
                return true;
            }
            return false;
        }
        public async Task<bool> StopService(string service, CancellationToken cancellationToken = default)
        {
            if ((await this.IsServiceRunning(service, cancellationToken)).HasValue)
            {
                await CommandService(service, "stop", cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<DateTime?> IsServiceRunning(string service, CancellationToken cancellationToken = default)
        {
            try
            {
                var activeRunnigString = "Active: ";
                var sinceString = " since";
                var isActiveText = "active (running)";
                var output = await CommandService(service, "status");
                var startIndex = output.IndexOf(activeRunnigString);
                var endOfLine = output.Substring(startIndex + activeRunnigString.Length).IndexOf(';');

                var activeStateIndex = output.Substring(startIndex + activeRunnigString.Length).IndexOf(sinceString);

                var activeText = output.Substring(startIndex + activeRunnigString.Length, activeStateIndex);
                var runningSincetext = output.Substring(startIndex + activeRunnigString.Length + activeStateIndex + sinceString.Length + 1 + 3, endOfLine - activeStateIndex - sinceString.Length - 1 - 8);
                var isActive = activeText.Contains(isActiveText);
                if (isActive &&
                    DateTime.TryParse(runningSincetext, out var running))
                {
                    return running;
                }
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
            }
            return null;
        }

        private Task<string> CommandService(string service, string command, CancellationToken cancellationToken = default)
            => this.RunProcess("service", $"{service} {command}", cancellationToken);

        private async Task<string> RunProcess(string filename, string args, CancellationToken cancellationToken)
        {
            try
            {
                var info = new ProcessStartInfo(filename, args)
                {
                    RedirectStandardOutput = true,
                };
                var proc = Process.Start(info);
                proc.Start();
                await proc.WaitForExitAsync(cancellationToken);
                return await proc.StandardOutput.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
                return string.Empty;
            }
        }
    }
    public interface ISystemManager
    {
        void ChangeStateInTime(string service, TimeSpan span, CancellationToken cancellationToken = default);
        Task<DateTime?> IsServiceRunning(string service, CancellationToken cancellationToken = default);
        Task<bool> StartService(string service, CancellationToken cancellationToken = default);
        Task<bool> StopService(string service, CancellationToken cancellationToken = default);
    }
}

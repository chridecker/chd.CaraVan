using System.Diagnostics;

namespace BlazorApp3
{
    public class SystemManager : ISystemManager
    {
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
            return null;
        }

        private Task<string> CommandService(string service, string command, CancellationToken cancellationToken = default)
            => this.RunProcess("service", $"{service} {command}", cancellationToken);

        private async Task<string> RunProcess(string filename, string args, CancellationToken cancellationToken)
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
    }
    public interface ISystemManager
    {
        Task<DateTime?> IsServiceRunning(string service, CancellationToken cancellationToken = default);
        Task<bool> StartService(string service, CancellationToken cancellationToken = default);
        Task<bool> StopService(string service, CancellationToken cancellationToken = default);
    }
}

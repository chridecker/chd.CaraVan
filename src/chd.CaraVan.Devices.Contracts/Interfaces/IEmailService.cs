
namespace chd.CaraVan.Devices.Contracts.Interfaces
{
public interface IEmailService
    {
        Task SendEmail(string to, string caption, string body, CancellationToken cancellationToken = default);
    }
}

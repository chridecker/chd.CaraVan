using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace chd.CaraVan.UI.Implementations
{
    public class EmailService : IEmailService
    {
        public async Task SendEmail(string to, string caption, string body, CancellationToken cancellationToken = default)
        {
            try
            {

                using var client = new SmtpClient("mail.gmx.net", 587)
                {
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential("chri_13@gmx.at", "35100"),
                    EnableSsl = true,
                    
                };

                var mail = new MailMessage("test@example.co", to)
                {
                    From = new MailAddress("test@example.com", "TestFromName"),
                    IsBodyHtml = false,
                    Body = body,
                    Subject = caption,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    SubjectEncoding = System.Text.Encoding.UTF8
                };
                await client.SendMailAsync(mail,cancellationToken);
            }

            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public interface IEmailService
    {
        Task SendEmail(string to, string caption, string body, CancellationToken cancellationToken = default);
    }
}

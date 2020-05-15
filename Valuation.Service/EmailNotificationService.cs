using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Valuation.Service
{
    public class EmailNotificationService : INotificationService
    {
        private readonly ILogger<EmailNotificationService> logger;
        private readonly IProviderService providerService;
        private readonly string serviceName;
        private readonly string fromAddress;
        private readonly string fromName;
        private readonly string toAddress;
        private readonly string smtpServerName;
        private readonly int port;

        public EmailNotificationService(ILogger<EmailNotificationService> logger, IProviderService providerService, string serviceName,
            string fromAddress, string fromName, string toAddress, string smtpServerName, int port)
        {
            this.logger = logger;
            this.providerService = providerService;
            this.serviceName = serviceName;
            this.fromAddress = fromAddress;
            this.fromName = fromName;
            this.toAddress = toAddress;
            this.smtpServerName = smtpServerName;
            this.port = port;
        }
        public async Task Send(string subject, string message)
        {
            var provider = await providerService.GetEmailProvider(serviceName);

            var smtp = new SmtpClient
            {
                Host = smtpServerName,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress, provider.Key)
            };

            var mailMessage = new MailMessage() { IsBodyHtml = true };
            mailMessage.From = new MailAddress(fromAddress, fromName); ;
            foreach (var to in toAddress.Split(new char[] { ',', ';', ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.To.Add(to);
            }
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            using (mailMessage)
            {
                using (smtp)
                {
                    await smtp.SendMailAsync(mailMessage);
                }
            }
            logger.LogInformation($"Email notication sent with subject: {subject} and message : {message}");
        }
    }
}

using System.Net.Mail;
using System.Net;
using ServerApp.DataAccessLayer;

namespace ServerApp.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IConfiguration configuration)
        {
            _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>()!;
        }
        public EmailService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                // Create the email message
                MailMessage mail = new MailMessage(_smtpSettings.FromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                };

                // Create the SMTP client
                SmtpClient client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                // Send the email
                client.Send(mail);
                Console.WriteLine($"Email sent to: {toEmail} at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }

}

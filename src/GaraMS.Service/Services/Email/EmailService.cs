using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendEmailAsync(string to, string subject, string statusChangeUrl)
        {
            var from = _configuration["EmailSettings:SmtpUser"];
            if (string.IsNullOrEmpty(from))
            {
                throw new ArgumentNullException(nameof(from), "Sender email address cannot be null or empty.");
            }

            var body = $@"
                <h2>Status Change Request</h2>
                <p>Please click the button below to change the status. This link can only be used once.</p>
                <a href='{statusChangeUrl}' style='padding: 10px 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px;'>Change Status</a>
            ";

            try
            {
                var mailMessage = new MailMessage(from, to, subject, body)
                {
                    IsBodyHtml = true
                };

                using (var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"], int.Parse(_configuration["EmailSettings:SmtpPort"])))
                {
                    smtpClient.Credentials = new NetworkCredential(
                        _configuration["EmailSettings:SmtpUser"],
                        _configuration["EmailSettings:SmtpPass"]
                    );

                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine("Email with button sent successfully to " + to);
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
                throw;
            }
        }
    }
}

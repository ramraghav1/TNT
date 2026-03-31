using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Bussiness.Services
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPass { get; set; } = string.Empty;
        public string FromName { get; set; } = "Suryantra";
        public string FromEmail { get; set; } = string.Empty;
        public string DemoNotifyEmail { get; set; } = "ramshranlamichhane@gmail.com";
    }

    public interface IEmailService
    {
        Task SendDemoRequestNotificationAsync(string requesterName, string requesterEmail, string? phone, string? companyName, string productInterest, string? message);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendDemoRequestNotificationAsync(
            string requesterName,
            string requesterEmail,
            string? phone,
            string? companyName,
            string productInterest,
            string? message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            email.To.Add(new MailboxAddress("Demo Team", _settings.DemoNotifyEmail));

            // CC the person who requested the demo
            if (!string.IsNullOrWhiteSpace(requesterEmail))
            {
                email.Cc.Add(new MailboxAddress(requesterName, requesterEmail));
            }

            email.Subject = $"New Demo Request: {productInterest} — {requesterName}";

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 24px; border-radius: 12px 12px 0 0;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 22px;'>New Demo Request Received</h1>
                    </div>
                    <div style='background: #f8fafc; padding: 24px; border: 1px solid #e2e8f0;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='padding: 10px 0; font-weight: bold; color: #475569; width: 140px;'>Name:</td>
                                <td style='padding: 10px 0; color: #1e293b;'>{requesterName}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px 0; font-weight: bold; color: #475569;'>Email:</td>
                                <td style='padding: 10px 0; color: #1e293b;'><a href='mailto:{requesterEmail}'>{requesterEmail}</a></td>
                            </tr>
                            <tr>
                                <td style='padding: 10px 0; font-weight: bold; color: #475569;'>Phone:</td>
                                <td style='padding: 10px 0; color: #1e293b;'>{phone ?? "—"}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px 0; font-weight: bold; color: #475569;'>Company:</td>
                                <td style='padding: 10px 0; color: #1e293b;'>{companyName ?? "—"}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px 0; font-weight: bold; color: #475569;'>Product:</td>
                                <td style='padding: 10px 0;'>
                                    <span style='background: #667eea; color: white; padding: 4px 12px; border-radius: 20px; font-size: 13px;'>{productInterest}</span>
                                </td>
                            </tr>
                            {(string.IsNullOrWhiteSpace(message) ? "" : $@"
                            <tr>
                                <td style='padding: 10px 0; font-weight: bold; color: #475569; vertical-align: top;'>Message:</td>
                                <td style='padding: 10px 0; color: #1e293b;'>{message}</td>
                            </tr>")}
                        </table>
                    </div>
                    <div style='background: #1e293b; padding: 16px 24px; border-radius: 0 0 12px 12px; text-align: center;'>
                        <p style='color: #94a3b8; margin: 0; font-size: 13px;'>Suryantra Technologies — Demo Request Notification</p>
                    </div>
                </div>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

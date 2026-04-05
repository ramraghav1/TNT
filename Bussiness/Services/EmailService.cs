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
        Task<bool> SendWelcomeEmailAsync(string toEmail, string fullName, string username, string password, string resetLink);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink);
        Task<bool> SendPasswordChangedNotificationAsync(string toEmail, string fullName);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;

            // Override from env vars if set (Render doesn't support __ in env names reliably)
            var envUser = Environment.GetEnvironmentVariable("SMTP_USER");
            var envPass = Environment.GetEnvironmentVariable("SMTP_PASS");
            if (!string.IsNullOrEmpty(envUser)) _settings.SmtpUser = envUser;
            if (!string.IsNullOrEmpty(envPass)) _settings.SmtpPass = envPass;
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
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string fullName, string username, string password, string resetLink)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                email.To.Add(new MailboxAddress(fullName, toEmail));
                email.Subject = "Welcome - Your Account Has Been Created";

                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 24px; border-radius: 12px 12px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 22px;'>Welcome to {_settings.FromName}!</h1>
                        </div>
                        <div style='background: #f8fafc; padding: 24px; border: 1px solid #e2e8f0;'>
                            <p style='color: #1e293b; font-size: 15px;'>Hello <strong>{fullName}</strong>,</p>
                            <p style='color: #475569;'>Your account has been successfully created. Here are your login credentials:</p>
                            <div style='background: #ffffff; padding: 16px; border-radius: 8px; margin: 16px 0; border: 1px solid #e2e8f0;'>
                                <p style='margin: 8px 0; color: #1e293b;'><strong>Username:</strong> {username}</p>
                                <p style='margin: 8px 0; color: #1e293b;'><strong>Temporary Password:</strong> <code style='background: #fee; padding: 4px 8px; border-radius: 4px; color: #dc2626;'>{password}</code></p>
                            </div>
                            <p style='color: #ef4444; font-weight: bold;'>⚠️ Please change your password after first login for security.</p>
                            <div style='margin: 24px 0;'>
                                <a href='{resetLink}' style='display: inline-block; background: #667eea; color: white; padding: 12px 24px; text-decoration: none; border-radius: 8px; font-weight: bold;'>Reset Password Now</a>
                            </div>
                        </div>
                        <div style='background: #1e293b; padding: 16px 24px; border-radius: 0 0 12px 12px; text-align: center;'>
                            <p style='color: #94a3b8; margin: 0; font-size: 13px;'>&copy; {_settings.FromName} — Welcome Email</p>
                        </div>
                    </div>"
                };

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send welcome email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                email.To.Add(new MailboxAddress(fullName, toEmail));
                email.Subject = "Password Reset Request";

                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background: linear-gradient(135deg, #f59e0b 0%, #ef4444 100%); padding: 24px; border-radius: 12px 12px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 22px;'>Password Reset Request</h1>
                        </div>
                        <div style='background: #f8fafc; padding: 24px; border: 1px solid #e2e8f0;'>
                            <p style='color: #1e293b; font-size: 15px;'>Hello <strong>{fullName}</strong>,</p>
                            <p style='color: #475569;'>We received a request to reset your password.</p>
                            <p style='color: #475569;'>Click the button below to reset your password:</p>
                            <div style='margin: 24px 0; text-align: center;'>
                                <a href='{resetLink}' style='display: inline-block; background: #ef4444; color: white; padding: 12px 24px; text-decoration: none; border-radius: 8px; font-weight: bold;'>Reset Password</a>
                            </div>
                            <p style='color: #64748b; font-size: 13px;'>This link will expire in 24 hours.</p>
                            <p style='color: #64748b; font-size: 13px;'>If you didn't request this password reset, please ignore this email or contact support if you have concerns.</p>
                        </div>
                        <div style='background: #1e293b; padding: 16px 24px; border-radius: 0 0 12px 12px; text-align: center;'>
                            <p style='color: #94a3b8; margin: 0; font-size: 13px;'>&copy; {_settings.FromName} — Password Reset</p>
                        </div>
                    </div>"
                };

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send password reset email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPasswordChangedNotificationAsync(string toEmail, string fullName)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                email.To.Add(new MailboxAddress(fullName, toEmail));
                email.Subject = "Password Changed Successfully";

                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background: linear-gradient(135deg, #10b981 0%, #059669 100%); padding: 24px; border-radius: 12px 12px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 22px;'>✓ Password Changed Successfully</h1>
                        </div>
                        <div style='background: #f8fafc; padding: 24px; border: 1px solid #e2e8f0;'>
                            <p style='color: #1e293b; font-size: 15px;'>Hello <strong>{fullName}</strong>,</p>
                            <p style='color: #475569;'>Your password has been changed successfully.</p>
                            <p style='color: #475569;'>If you didn't make this change, please contact support immediately.</p>
                            <div style='margin: 24px 0; padding: 16px; background: #fef2f2; border-left: 4px solid #ef4444; border-radius: 4px;'>
                                <p style='color: #991b1b; margin: 0; font-size: 13px;'><strong>Security Alert:</strong> If you did not authorize this change, your account may be compromised.</p>
                            </div>
                        </div>
                        <div style='background: #1e293b; padding: 16px 24px; border-radius: 0 0 12px 12px; text-align: center;'>
                            <p style='color: #94a3b8; margin: 0; font-size: 13px;'>&copy; {_settings.FromName} — Security Notification</p>
                        </div>
                    </div>"
                };

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send password changed notification: {ex.Message}");
                return false;
            }
        }
    }
}

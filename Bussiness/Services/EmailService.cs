using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace Bussiness.Services
{
    public class CompanySettings
    {
        public string Name { get; set; } = "TNT Nepal";
        public string Address { get; set; } = "Kathmandu, Nepal";
        public string Phone { get; set; } = "+977-1-XXXXXXX";
        public string Email { get; set; } = "info@tntnepal.com";
        public string Website { get; set; } = "www.tntnepal.com";
        public string PanVat { get; set; } = "";
    }

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

    public class ProposalEmailDayInfo
    {
        public int DayNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Accommodation { get; set; }
        public string? Transport { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool LunchIncluded { get; set; }
        public bool DinnerIncluded { get; set; }
        public List<string> Activities { get; set; } = new();
        public decimal DailyCost { get; set; }
    }

    public interface IEmailService
    {
        Task SendDemoRequestNotificationAsync(string requesterName, string requesterEmail, string? phone, string? companyName, string productInterest, string? message);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string fullName, string username, string password, string resetLink);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink);
        Task<bool> SendPasswordChangedNotificationAsync(string toEmail, string fullName);
        Task<bool> SendItineraryProposalEmailAsync(
            string toEmail, string travelerName, string? travelerPhone,
            string itineraryTitle, long proposalId,
            string proposalUrl, string feedbackUrl,
            decimal totalAmount, int totalDays,
            DateTime startDate, DateTime endDate,
            DateTime expiresAt, string? notes,
            List<ProposalEmailDayInfo> days,
            string? ccEmail = null);
        Task<bool> SendPaymentVerifiedEmailAsync(string toEmail, string travelerName, string itineraryTitle, bool approved, string? remarks);
        Task<(bool success, string message)> SendTestEmailAsync(string toEmail);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly CompanySettings _company;

        public EmailService(IOptions<EmailSettings> settings, IOptions<CompanySettings> companySettings)
        {
            _settings = settings.Value;
            _company = companySettings.Value;

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

        public async Task<bool> SendItineraryProposalEmailAsync(
            string toEmail, string travelerName, string? travelerPhone,
            string itineraryTitle, long proposalId,
            string proposalUrl, string feedbackUrl,
            decimal totalAmount, int totalDays,
            DateTime startDate, DateTime endDate,
            DateTime expiresAt, string? notes,
            List<ProposalEmailDayInfo> days,
            string? ccEmail = null)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                email.To.Add(new MailboxAddress(travelerName, toEmail));

                if (!string.IsNullOrWhiteSpace(ccEmail))
                    email.Cc.Add(new MailboxAddress("Admin", ccEmail));

                string quotationNo = $"QTN-{DateTime.Now:yyyy}-{proposalId:D3}";
                email.Subject = $"Tour Quotation {quotationNo}: {itineraryTitle}";

                // Build day-wise itinerary rows
                var dayRows = new StringBuilder();
                foreach (var day in days)
                {
                    var desc = day.Title ?? day.Location ?? "";
                    var activities = day.Activities.Count > 0 ? string.Join(", ", day.Activities) : "";
                    var summary = !string.IsNullOrWhiteSpace(activities) ? $"{desc} — {activities}" : desc;
                    var descriptionHtml = !string.IsNullOrWhiteSpace(day.Description)
                        ? $"<br/><span style='font-size:12px;color:#6b7280;'>{day.Description}</span>"
                        : "";

                    dayRows.Append($@"
                        <tr>
                            <td style='padding:8px 12px;border-bottom:1px solid #e5e7eb;color:#4F46E5;font-weight:600;white-space:nowrap;vertical-align:top;'>Day {day.DayNumber}</td>
                            <td style='padding:8px 12px;border-bottom:1px solid #e5e7eb;color:#374151;'>{summary}{descriptionHtml}</td>
                            <td style='padding:8px 12px;border-bottom:1px solid #e5e7eb;color:#374151;text-align:center;'>{(day.Location ?? "—")}</td>
                        </tr>");
                }

                // Derive inclusions from day data
                bool hasAccommodation = days.Any(d => !string.IsNullOrWhiteSpace(d.Accommodation));
                bool hasTransport = days.Any(d => !string.IsNullOrWhiteSpace(d.Transport));
                bool hasBreakfast = days.Any(d => d.BreakfastIncluded);
                bool hasLunch = days.Any(d => d.LunchIncluded);
                bool hasDinner = days.Any(d => d.DinnerIncluded);

                var inclusions = new List<string>();
                if (hasAccommodation) inclusions.Add("Hotel accommodation as per itinerary");
                if (hasBreakfast) inclusions.Add("Daily breakfast");
                if (hasLunch) inclusions.Add("Lunch as per itinerary");
                if (hasDinner) inclusions.Add("Dinner as per itinerary");
                if (hasTransport) inclusions.Add("Private transport as per itinerary");
                inclusions.Add("Airport pickup & drop");
                inclusions.Add("Professional guide service");
                inclusions.Add("All applicable entry fees");

                var exclusions = new List<string>
                {
                    "International airfare",
                    !hasLunch ? "Lunch" : "",
                    !hasDinner ? "Dinner" : "",
                    "Personal expenses & shopping",
                    "Travel insurance",
                    "Tips & gratuities",
                    "Any items not mentioned in inclusions"
                };
                exclusions.RemoveAll(string.IsNullOrWhiteSpace);

                string inclusionList = string.Join("", inclusions.Select(i => $"<tr><td style='padding:4px 0 4px 0;color:#374151;font-size:13px;'>✓ {i}</td></tr>"));
                string exclusionList = string.Join("", exclusions.Select(e => $"<tr><td style='padding:4px 0 4px 0;color:#374151;font-size:13px;'>✗ {e}</td></tr>"));

                // Cost summary (total only, no per-day breakdown)
                var costRows = $@"
                        <tr>
                            <td style='padding:6px 12px;border-bottom:1px solid #f3f4f6;color:#374151;font-size:13px;'>{itineraryTitle} ({totalDays} Days)</td>
                            <td style='padding:6px 12px;border-bottom:1px solid #f3f4f6;color:#374151;font-size:13px;text-align:right;'>NPR {totalAmount:N2}</td>
                        </tr>";

                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"
<div style='font-family:Segoe UI,Arial,sans-serif;max-width:700px;margin:0 auto;color:#1f2937;'>

    <!-- COMPANY HEADER -->
    <table style='width:100%;border-bottom:3px solid #4F46E5;padding-bottom:16px;margin-bottom:0;'>
        <tr>
            <td style='padding:20px 0;'>
                <h1 style='margin:0;font-size:24px;color:#4F46E5;font-weight:700;'>{_company.Name}</h1>
                <p style='margin:4px 0 0;font-size:12px;color:#6b7280;'>{_company.Address}</p>
                <p style='margin:2px 0 0;font-size:12px;color:#6b7280;'>Phone: {_company.Phone} | Email: {_company.Email}</p>
                <p style='margin:2px 0 0;font-size:12px;color:#6b7280;'>{_company.Website}{(string.IsNullOrWhiteSpace(_company.PanVat) ? "" : $" | PAN/VAT: {_company.PanVat}")}</p>
            </td>
            <td style='padding:20px 0;text-align:right;vertical-align:top;'>
                <span style='display:inline-block;background:#4F46E5;color:#fff;padding:8px 20px;border-radius:4px;font-size:14px;font-weight:600;letter-spacing:1px;'>QUOTATION</span>
            </td>
        </tr>
    </table>

    <!-- QUOTATION DETAILS -->
    <table style='width:100%;margin:20px 0 16px;'>
        <tr>
            <td style='width:50%;vertical-align:top;'>
                <table style='font-size:13px;'>
                    <tr><td style='color:#6b7280;padding:3px 12px 3px 0;'>Quotation No:</td><td style='color:#1f2937;font-weight:600;'>{quotationNo}</td></tr>
                    <tr><td style='color:#6b7280;padding:3px 12px 3px 0;'>Date:</td><td style='color:#1f2937;'>{DateTime.Now:dd-MMM-yyyy}</td></tr>
                    <tr><td style='color:#6b7280;padding:3px 12px 3px 0;'>Valid Until:</td><td style='color:#dc2626;font-weight:600;'>{expiresAt:dd-MMM-yyyy}</td></tr>
                </table>
            </td>
            <td style='width:50%;vertical-align:top;'>
                <table style='font-size:13px;'>
                    <tr><td style='color:#6b7280;padding:3px 12px 3px 0;'>Client Name:</td><td style='color:#1f2937;font-weight:600;'>{travelerName}</td></tr>
                    <tr><td style='color:#6b7280;padding:3px 12px 3px 0;'>Email:</td><td style='color:#1f2937;'>{toEmail}</td></tr>
                    <tr><td style='color:#6b7280;padding:3px 12px 3px 0;'>Phone:</td><td style='color:#1f2937;'>{travelerPhone ?? "—"}</td></tr>
                </table>
            </td>
        </tr>
    </table>

    <!-- TOUR TITLE -->
    <div style='background:#f0f0ff;border-left:4px solid #4F46E5;padding:14px 18px;margin:16px 0;border-radius:0 6px 6px 0;'>
        <h2 style='margin:0;font-size:18px;color:#1f2937;'>{itineraryTitle}</h2>
        <p style='margin:6px 0 0;font-size:13px;color:#6b7280;'>{totalDays} Days &nbsp;|&nbsp; {startDate:dd MMM yyyy} — {endDate:dd MMM yyyy}</p>
    </div>

    <!-- DAY-WISE ITINERARY -->
    <h3 style='font-size:15px;color:#4F46E5;margin:24px 0 10px;border-bottom:1px solid #e5e7eb;padding-bottom:6px;'>Day-wise Itinerary</h3>
    <table style='width:100%;border-collapse:collapse;'>
        <thead>
            <tr style='background:#f9fafb;'>
                <th style='padding:8px 12px;text-align:left;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;border-bottom:2px solid #e5e7eb;width:70px;'>Day</th>
                <th style='padding:8px 12px;text-align:left;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Description</th>
                <th style='padding:8px 12px;text-align:center;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;border-bottom:2px solid #e5e7eb;width:120px;'>Location</th>
            </tr>
        </thead>
        <tbody>
            {dayRows}
        </tbody>
    </table>

    <!-- INCLUSIONS & EXCLUSIONS -->
    <table style='width:100%;margin:24px 0 0;'>
        <tr>
            <td style='width:50%;vertical-align:top;padding-right:12px;'>
                <h3 style='font-size:14px;color:#059669;margin:0 0 8px;'>✓ Inclusions</h3>
                <table style='width:100%;'>{inclusionList}</table>
            </td>
            <td style='width:50%;vertical-align:top;padding-left:12px;'>
                <h3 style='font-size:14px;color:#dc2626;margin:0 0 8px;'>✗ Exclusions</h3>
                <table style='width:100%;'>{exclusionList}</table>
            </td>
        </tr>
    </table>

    <!-- TOTAL AMOUNT -->
    <div style='background:#f0f0ff;border:2px solid #4F46E5;border-radius:8px;padding:16px 20px;margin:28px 0 0;display:flex;justify-content:space-between;align-items:center;'>
        <span style='font-size:15px;font-weight:700;color:#1f2937;'>Total Amount</span>
        <span style='font-size:20px;font-weight:700;color:#4F46E5;'>NPR {totalAmount:N2}</span>
    </div>

    {(string.IsNullOrWhiteSpace(notes) ? "" : $@"
    <div style='margin:16px 0;padding:10px 14px;background:#fffbeb;border-left:3px solid #f59e0b;border-radius:0 4px 4px 0;'>
        <p style='margin:0;font-size:13px;color:#92400e;'><strong>Note:</strong> {notes}</p>
    </div>")}

    <!-- PAYMENT TERMS -->
    <h3 style='font-size:15px;color:#4F46E5;margin:28px 0 10px;border-bottom:1px solid #e5e7eb;padding-bottom:6px;'>Payment Terms</h3>
    <table style='font-size:13px;color:#374151;'>
        <tr><td style='padding:3px 0;'>• 30% advance for booking confirmation</td></tr>
        <tr><td style='padding:3px 0;'>• 70% before or on arrival</td></tr>
        <tr><td style='padding:3px 0;'>• Payment methods: Bank transfer / Cash / Online</td></tr>
    </table>

    <!-- CANCELLATION POLICY -->
    <h3 style='font-size:15px;color:#4F46E5;margin:24px 0 10px;border-bottom:1px solid #e5e7eb;padding-bottom:6px;'>Cancellation Policy</h3>
    <table style='font-size:13px;color:#374151;'>
        <tr><td style='padding:3px 0;'>• 30+ days before departure: Full refund (minus admin fee)</td></tr>
        <tr><td style='padding:3px 0;'>• 15–30 days before: 50% refund</td></tr>
        <tr><td style='padding:3px 0;'>• Less than 15 days: No refund</td></tr>
    </table>

    <!-- TERMS & CONDITIONS -->
    <h3 style='font-size:15px;color:#4F46E5;margin:24px 0 10px;border-bottom:1px solid #e5e7eb;padding-bottom:6px;'>Terms &amp; Conditions</h3>
    <table style='font-size:12px;color:#6b7280;'>
        <tr><td style='padding:2px 0;'>• Itinerary may change due to weather or unforeseen local conditions.</td></tr>
        <tr><td style='padding:2px 0;'>• Hotel accommodation is subject to availability and may be substituted with equivalent options.</td></tr>
        <tr><td style='padding:2px 0;'>• Additional services not listed above will be charged separately.</td></tr>
        <tr><td style='padding:2px 0;'>• This quotation is valid until the date mentioned above.</td></tr>
    </table>

    <!-- ACTION BUTTONS -->
    <div style='margin:28px 0;text-align:center;'>
        <a href='{proposalUrl}' style='display:inline-block;background:#4F46E5;color:#ffffff;padding:12px 32px;text-decoration:none;border-radius:6px;font-weight:600;font-size:14px;'>View Full Itinerary &amp; Accept</a>
        <span style='display:inline-block;width:12px;'></span>
        <a href='{feedbackUrl}' style='display:inline-block;background:#ffffff;color:#4F46E5;padding:12px 24px;text-decoration:none;border-radius:6px;font-weight:600;font-size:13px;border:1px solid #4F46E5;'>Request Changes</a>
    </div>

    <!-- FOOTER -->
    <div style='border-top:2px solid #e5e7eb;padding:16px 0;margin-top:12px;text-align:center;'>
        <p style='margin:0;font-size:12px;color:#6b7280;'>Prepared by <strong>{_company.Name}</strong></p>
        <p style='margin:4px 0 0;font-size:11px;color:#9ca3af;'>{_company.Address} | {_company.Phone} | {_company.Email}</p>
        <p style='margin:8px 0 0;font-size:11px;color:#9ca3af;'>This is a system-generated quotation. For any queries, please contact us.</p>
    </div>

</div>"
                };

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                var response = await smtp.SendAsync(email);
                Console.WriteLine($"[EMAIL-DEBUG] Proposal email SMTP Response: {response}");
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send itinerary proposal email: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[ERROR] Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                return false;
            }
        }

        public async Task<(bool success, string message)> SendTestEmailAsync(string toEmail)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                email.To.Add(new MailboxAddress("Test", toEmail));
                email.Subject = "TNT Email Test";
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $"<h2>Email Test</h2><p>This is a test email sent at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.</p><p>From: {_settings.FromEmail}<br/>SMTP: {_settings.SmtpHost}:{_settings.SmtpPort}<br/>User: {_settings.SmtpUser}</p>"
                };

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                var response = await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return (true, $"SMTP accepted. Response: {response}");
            }
            catch (Exception ex)
            {
                return (false, $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        public async Task<bool> SendPaymentVerifiedEmailAsync(
            string toEmail, string travelerName, string itineraryTitle, bool approved, string? remarks)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                email.To.Add(new MailboxAddress(travelerName, toEmail));

                string statusText = approved ? "Verified & Confirmed" : "Requires Attention";
                string gradient = approved ? "#10b981, #059669" : "#f59e0b, #ef4444";
                string icon = approved ? "&#10003;" : "&#9888;";
                string body = approved
                    ? "Your payment has been verified and your booking is now confirmed! We look forward to hosting you."
                    : $"Your payment could not be verified.{(string.IsNullOrWhiteSpace(remarks) ? "" : $" Reason: {remarks}")} Please contact us or try again.";

                email.Subject = $"Payment {statusText} — {itineraryTitle}";
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background: linear-gradient(135deg, {gradient}); padding: 24px; border-radius: 12px 12px 0 0;'>
                            <h1 style='color: #fff; margin: 0; font-size: 22px;'>{icon} Payment {statusText}</h1>
                        </div>
                        <div style='background: #f8fafc; padding: 24px; border: 1px solid #e2e8f0;'>
                            <p style='color: #1e293b; font-size: 15px;'>Hello <strong>{travelerName}</strong>,</p>
                            <p style='color: #475569;'>{body}</p>
                            <p style='color: #475569;'>Itinerary: <strong>{itineraryTitle}</strong></p>
                        </div>
                        <div style='background: #1e293b; padding: 16px 24px; border-radius: 0 0 12px 12px; text-align: center;'>
                            <p style='color: #94a3b8; margin: 0; font-size: 13px;'>&copy; {_settings.FromName}</p>
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
                Console.WriteLine($"[ERROR] Failed to send payment verified email: {ex.Message}");
                return false;
            }
        }
    }
}

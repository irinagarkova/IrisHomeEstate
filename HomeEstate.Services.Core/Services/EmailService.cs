// Create this file: HomeEstate.Services.Core/Services/EmailService.cs
using HomeEstate.Services.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace HomeEstate.Services.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    emailSettings["FromName"],
                    emailSettings["FromEmail"]
                ));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Connect to Gmail SMTP server
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"]),
                    SecureSocketOptions.StartTls
                );

                // Authenticate
                await client.AuthenticateAsync(
                    emailSettings["SmtpUsername"],
                    emailSettings["SmtpPassword"]
                );

                // Send email
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Възстановяване на парола - Iris Home Estate";
            var body = GetPasswordResetEmailTemplate(resetLink);

            return await SendEmailAsync(toEmail, subject, body, true);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Добре дошъл в Iris Home Estate!";
            var body = GetWelcomeEmailTemplate(userName);

            return await SendEmailAsync(toEmail, subject, body, true);
        }

        public async Task<bool> SendContactFormEmailAsync(string fromEmail, string fromName, string subject, string message)
        {
            var emailSubject = $"Контактна форма: {subject}";
            var body = GetContactFormEmailTemplate(fromEmail, fromName, subject, message);

            // Send to admin email
            var adminEmail = _configuration["EmailSettings:AdminEmail"] ?? _configuration["EmailSettings:FromEmail"];
            return await SendEmailAsync(adminEmail, emailSubject, body, true);
        }

        private string GetPasswordResetEmailTemplate(string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Възстановяване на парола</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 0; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
        .content {{ padding: 30px; }}
        .button {{ display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; margin: 20px 0; }}
        .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 14px; }}
        .warning {{ background: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🏠 Iris Home Estate</h1>
            <h2>Възстановяване на парола</h2>
        </div>
        <div class='content'>
            <p>Здравейте,</p>
            <p>Получихме заявка за възстановяване на паролата за вашия акаунт в Iris Home Estate.</p>
            
            <div class='warning'>
                <strong>⚠️ Важно:</strong> Ако не сте заявили възстановяване на парола, моля игнорирайте този имейл. Вашата парола няма да бъде променена.
            </div>
            
            <p>За да създадете нова парола, кликнете върху бутона по-долу:</p>
            
            <div style='text-align: center;'>
                <a href='{resetLink}' class='button'>Възстанови парола</a>
            </div>
            
            <p>Или копирайте и поставете този линк в браузъра си:</p>
            <p style='word-break: break-all; background: #f8f9fa; padding: 10px; border-radius: 5px;'>{resetLink}</p>
            
            <p><strong>Този линк е валиден за 24 часа</strong> от момента на получаване на този имейл.</p>
            
            <p>Ако имате въпроси, не се колебайте да се свържете с нас.</p>
            
            <p>С уважение,<br>
            Екипът на Iris Home Estate</p>
        </div>
        <div class='footer'>
            <p>© 2025 Iris Home Estate. Всички права запазени.</p>
            <p>София, България | info@irishomeestate.com | +359 881 234 567</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetWelcomeEmailTemplate(string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Добре дошъл</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 0; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
        .content {{ padding: 30px; }}
        .feature {{ background: #f8f9fa; padding: 20px; margin: 15px 0; border-radius: 8px; border-left: 4px solid #667eea; }}
        .button {{ display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; margin: 20px 0; }}
        .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🏠 Iris Home Estate</h1>
            <h2>Добре дошъл, {userName}!</h2>
        </div>
        <div class='content'>
            <p>Благодарим ви, че се присъединихте към Iris Home Estate - вашия надежден партньор в света на недвижимите имоти!</p>
            
            <h3>Какво можете да правите с вашия акаунт:</h3>
            
            <div class='feature'>
                <h4>🔍 Търсене на имоти</h4>
                <p>Преглеждайте хиляди имоти за продажба и наем с усъвършенствани филтри за търсене.</p>
            </div>
            
            <div class='feature'>
                <h4>❤️ Любими имоти</h4>
                <p>Запазвайте имотите, които ви харесват, за по-лесен достъп по-късно.</p>
            </div>
            
            <div class='feature'>
                <h4>🏡 Публикуване на обяви</h4>
                <p>Публикувайте вашите имоти за продажба или наем с професионални снимки и описания.</p>
            </div>
            
            <div class='feature'>
                <h4>📊 Управление на имоти</h4>
                <p>Следете статистиките на вашите обяви и управлявайте ги лесно.</p>
            </div>
            
            <div style='text-align: center;'>
                <a href='https://localhost:7266' class='button'>Започнете да разглеждате</a>
            </div>
            
            <p>Ако имате въпроси или се нуждаете от помощ, нашият екип е винаги готов да ви помогне.</p>
            
            <p>Успех в търсенето на перфектния имот!</p>
            
            <p>С уважение,<br>
            Екипът на Iris Home Estate</p>
        </div>
        <div class='footer'>
            <p>© 2025 Iris Home Estate. Всички права запазени.</p>
            <p>София, България | info@irishomeestate.com | +359 881 234 567</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetContactFormEmailTemplate(string fromEmail, string fromName, string subject, string message)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Контактна форма</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ border-bottom: 2px solid #667eea; padding-bottom: 20px; margin-bottom: 20px; }}
        .field {{ margin: 15px 0; }}
        .label {{ font-weight: bold; color: #333; }}
        .value {{ background: #f8f9fa; padding: 10px; border-radius: 5px; margin-top: 5px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>📧 Нова заявка от контактната форма</h2>
        </div>
        
        <div class='field'>
            <div class='label'>От:</div>
            <div class='value'>{fromName} ({fromEmail})</div>
        </div>
        
        <div class='field'>
            <div class='label'>Тема:</div>
            <div class='value'>{subject}</div>
        </div>
        
        <div class='field'>
            <div class='label'>Съобщение:</div>
            <div class='value'>{message}</div>
        </div>
        
        <div class='field'>
            <div class='label'>Дата:</div>
            <div class='value'>{DateTime.Now:dd.MM.yyyy HH:mm}</div>
        </div>
    </div>
</body>
</html>";
        }
    }
}
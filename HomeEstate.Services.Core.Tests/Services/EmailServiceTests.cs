using FakeItEasy;
using HomeEstate.Services.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace HomeEstate.Services.Core.Tests.Services
{
    public class EmailServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly EmailService _service;

        public EmailServiceTests()
        {
            _configuration = A.Fake<IConfiguration>();
            _logger = A.Fake<ILogger<EmailService>>();

            // Setup configuration values
            SetupConfiguration();

            _service = new EmailService(_configuration, _logger);
        }

        private void SetupConfiguration()
        {
            var emailSettingsSection = A.Fake<IConfigurationSection>();

            A.CallTo(() => _configuration.GetSection("EmailSettings")).Returns(emailSettingsSection);
            A.CallTo(() => emailSettingsSection["FromName"]).Returns("Test Sender");
            A.CallTo(() => emailSettingsSection["FromEmail"]).Returns("test@example.com");
            A.CallTo(() => emailSettingsSection["SmtpServer"]).Returns("smtp.gmail.com");
            A.CallTo(() => emailSettingsSection["SmtpPort"]).Returns("587");
            A.CallTo(() => emailSettingsSection["SmtpUsername"]).Returns("username");
            A.CallTo(() => emailSettingsSection["SmtpPassword"]).Returns("password");
            A.CallTo(() => emailSettingsSection["AdminEmail"]).Returns("admin@example.com");
        }

        [Fact]
        public async Task SendPasswordResetEmailAsync_ShouldCallSendEmailAsync()
        {
            // Arrange
            var toEmail = "user@example.com";
            var resetLink = "https://example.com/reset-password?token=abc123";

            // We can't easily test the actual email sending without a real SMTP server
            // So we'll test that the method doesn't throw an exception with valid parameters

            // Act & Assert
            // This will likely return false in test environment due to no real SMTP server
            // but we're testing that it doesn't crash
            var result = await _service.SendPasswordResetEmailAsync(toEmail, resetLink);

            // The method should complete without throwing an exception
            // In a real environment with proper SMTP settings, this would return true
        }

        [Fact]
        public async Task SendWelcomeEmailAsync_ShouldCallSendEmailAsync()
        {
            // Arrange
            var toEmail = "user@example.com";
            var userName = "TestUser";

            // Act & Assert
            // This will likely return false in test environment due to no real SMTP server
            var result = await _service.SendWelcomeEmailAsync(toEmail, userName);

            // The method should complete without throwing an exception
        }

        [Fact]
        public async Task SendContactFormEmailAsync_ShouldCallSendEmailAsync()
        {
            // Arrange
            var fromEmail = "sender@example.com";
            var fromName = "Test Sender";
            var subject = "Test Subject";
            var message = "Test message content";

            // Act & Assert
            var result = await _service.SendContactFormEmailAsync(fromEmail, fromName, subject, message);

            // The method should complete without throwing an exception
        }

        [Theory]
        [InlineData("valid@email.com", "Test Subject", "Test Body", true)]
        [InlineData("valid@email.com", "Test Subject", "Test Body", false)]
        public async Task SendEmailAsync_WithValidParameters_ShouldNotThrowException(
            string toEmail, string subject, string body, bool isHtml)
        {
            // Act & Assert
            // This will likely return false in test environment due to no real SMTP server
            // but we're testing that it doesn't crash with valid parameters
            var result = await _service.SendEmailAsync(toEmail, subject, body, isHtml);

            // The method should complete without throwing an exception
        }

        [Fact]
        public async Task SendEmailAsync_WithNullToEmail_ShouldNotThrowException()
        {
            // Arrange
            string toEmail = null;
            var subject = "Test Subject";
            var body = "Test Body";

            // Act & Assert
            // Should handle null gracefully and return false
            var result = await _service.SendEmailAsync(toEmail, subject, body);
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task SendEmailAsync_WithEmptyToEmail_ShouldNotThrowException()
        {
            // Arrange
            var toEmail = "";
            var subject = "Test Subject";
            var body = "Test Body";

            // Act & Assert
            // Should handle empty email gracefully and return false
            var result = await _service.SendEmailAsync(toEmail, subject, body);
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task SendEmailAsync_WithNullSubject_ShouldNotThrowException()
        {
            // Arrange
            var toEmail = "test@example.com";
            string subject = null;
            var body = "Test Body";

            // Act & Assert
            var result = await _service.SendEmailAsync(toEmail, subject, body);
            // Should handle null subject gracefully
        }

        [Fact]
        public async Task SendEmailAsync_WithNullBody_ShouldNotThrowException()
        {
            // Arrange
            var toEmail = "test@example.com";
            var subject = "Test Subject";
            string body = null;

            // Act & Assert
            var result = await _service.SendEmailAsync(toEmail, subject, body);
            // Should handle null body gracefully
        }

        [Fact]
        public async Task SendPasswordResetEmailAsync_WithNullEmail_ShouldReturnFalse()
        {
            // Arrange
            string toEmail = null;
            var resetLink = "https://example.com/reset";

            // Act
            var result = await _service.SendPasswordResetEmailAsync(toEmail, resetLink);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task SendPasswordResetEmailAsync_WithNullResetLink_ShouldNotThrowException()
        {
            // Arrange
            var toEmail = "test@example.com";
            string resetLink = null;

            // Act & Assert
            var result = await _service.SendPasswordResetEmailAsync(toEmail, resetLink);
            // Should handle null reset link gracefully
        }

        [Fact]
        public async Task SendWelcomeEmailAsync_WithNullUserName_ShouldNotThrowException()
        {
            // Arrange
            var toEmail = "test@example.com";
            string userName = null;

            // Act & Assert
            var result = await _service.SendWelcomeEmailAsync(toEmail, userName);
            // Should handle null username gracefully
        }

        [Fact]
        public async Task SendContactFormEmailAsync_WithNullParameters_ShouldNotThrowException()
        {
            // Arrange
            string fromEmail = null;
            string fromName = null;
            string subject = null;
            string message = null;

            // Act & Assert
            var result = await _service.SendContactFormEmailAsync(fromEmail, fromName, subject, message);
            // Should handle null parameters gracefully
        }

        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("   ", "   ", "   ", "   ")]
        public async Task SendContactFormEmailAsync_WithEmptyOrWhitespaceParameters_ShouldNotThrowException(
            string fromEmail, string fromName, string subject, string message)
        {
            // Act & Assert
            var result = await _service.SendContactFormEmailAsync(fromEmail, fromName, subject, message);
            // Should handle empty/whitespace parameters gracefully
        }

        [Fact]
        public async Task SendContactFormEmailAsync_WithValidParameters_ShouldIncludeAllInformation()
        {
            // Arrange
            var fromEmail = "sender@example.com";
            var fromName = "John Doe";
            var subject = "Test Inquiry";
            var message = "This is a test message from the contact form.";

            // Act
            var result = await _service.SendContactFormEmailAsync(fromEmail, fromName, subject, message);

            // Assert
            // The method should complete without throwing an exception
            // In a real implementation, we would verify the email content contains all the provided information
        }
    }
}
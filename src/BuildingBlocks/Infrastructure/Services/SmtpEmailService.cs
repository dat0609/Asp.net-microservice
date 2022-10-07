using Contracts.Services;
using Infrastructure.Configurations;
using MimeKit;
using Shared.Services.Email;
using ILogger = Serilog.ILogger;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Infrastructure.Services;

public class SmtpEmailService : ISmtpEmailService
{
    private readonly ILogger _logger;
    private readonly SMTPEmailSetting _settings;
    private readonly SmtpClient _smtpClient;

    public SmtpEmailService(ILogger logger, SMTPEmailSetting settings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _smtpClient = new SmtpClient();
    }

    public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        var emailMessage = new MimeMessage
        {
            Sender = new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From),
            Subject = request.Subject,
            Body = new BodyBuilder
            {
                HtmlBody = request.Body
            }.ToMessageBody()
        };

        if (request.ToAddresses.Any())
        {
            foreach (var toAddress in request.ToAddresses)
            {
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));
            }
        }
        else
        {
            var toAddress = request.ToAddress;
            emailMessage.To.Add(MailboxAddress.Parse(toAddress));
        }

        try
        {
            await _smtpClient.ConnectAsync(_settings.SMTPServer, _settings.Port,
                _settings.UseSsl, cancellationToken);
            await _smtpClient.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            await _smtpClient.SendAsync(emailMessage, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
        }
        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
            _smtpClient.Dispose();
        }
    }
}
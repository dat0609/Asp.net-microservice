using Contracts.ScheduleJobs;
using Contracts.Services;
using Hangfire.API.Service.Interface;
using Shared.Services.Email;

namespace Hangfire.API.Service;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IScheduleJobService _jobService;
    private readonly ISmtpEmailService _emailService;
    private readonly Serilog.ILogger _logger;
    
    public BackgroundJobService(IScheduleJobService jobService, ISmtpEmailService emailService, Serilog.ILogger logger)
    {
        _jobService = jobService;
        _emailService = emailService;
        _logger = logger;
    }

    public IScheduleJobService ScheduledJobService => _jobService;

    public IScheduleJobService ScheduleJobService { get; }

    public string SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt)
    {
        var emailRequest = new MailRequest
        {
            ToAddress = email,
            Body = emailContent,
            Subject = subject
        };

        try
        {
            var jobId = _jobService.Schedule(() => _emailService.SendEmailAsync(emailRequest, CancellationToken.None), enqueueAt);            
            _logger.Information($"Sent email to {email} with subject: {subject} - Job Id: {jobId}");
            
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.Error($"failed due to an error with the email service: {ex.Message}");
        }

        return null;
    }
}
using Contracts.ScheduleJobs;

namespace Hangfire.API.Service.Interface;

public interface IBackgroundJobService
{
    IScheduleJobService ScheduleJobService { get; }
    string SendEmailContent(string email, string subject, string content, DateTimeOffset dateTime);
}
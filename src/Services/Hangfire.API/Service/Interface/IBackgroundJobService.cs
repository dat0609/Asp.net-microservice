namespace Hangfire.API.Service.Interface;

public interface IBackgroundJobService
{
    string SendEmailContent(string email, string subject, string content, DateTimeOffset dateTime);
}
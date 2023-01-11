using Contracts.Configurations;
using Contracts.ScheduleJobs;
using Contracts.Services;
using Hangfire.API.Service;
using Hangfire.API.Service.Interface;
using Infrastructure.Configurations;
using Infrastructure.ScheduleJob;
using Infrastructure.Services;
using Shared.Configurations;

namespace Hangfire.API.Extensions;

public static class ServiceExtension
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var hangFireSettings = configuration.GetSection(nameof(HangfireSettings))
            .Get<HangfireSettings>();
        services.AddSingleton(hangFireSettings);
        
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            .Get<SMTPEmailSetting>();
        services.AddSingleton(emailSettings);

        return services;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
        => services.AddTransient<IScheduleJobService, HangfireService>()
            .AddScoped<ISmtpEmailService, SmtpEmailService>()
            .AddScoped<IBackgroundJobService, BackgroundJobService>()
    ;
}
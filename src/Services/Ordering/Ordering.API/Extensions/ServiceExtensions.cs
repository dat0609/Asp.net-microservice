using EventBus.Messages.IntegrationEvents.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ordering.API.Application.IntegrationEvents.EventHandler;
using Shared.Configurations;

namespace Ordering.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            .Get<SMTPEmailSetting>();
        services.AddSingleton(emailSettings);

        var eventBus = services.GetOption<EventBusSettings>("EventBusSettings");
        services.AddSingleton(eventBus);

        return services;
    }

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOption<EventBusSettings>("EventBusSettings");

        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        var mqConnection = new Uri(settings.HostAddress!);
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<BasketCheckoutEventHandler>();
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(mqConnection);
                config.ConfigureEndpoints(context);
            });
        });
    }
}
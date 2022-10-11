using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Interfaces;
using Infrastructure.Common;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var eventBus = configuration.GetSection(nameof(EventBusSettings))
            .Get<EventBusSettings>();
        services.AddSingleton(eventBus);

        return services;
    }

    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = services.GetOption<CacheSettings>("CacheSettings").ConnectionString;
        if (string.IsNullOrEmpty(redisConnectionString))
            throw new ArgumentException("Redis Connection string is not configured!");
        // Redis Configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializerService, SerializerService>();

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOption<EventBusSettings>("EventBusSettings");

        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        var mqConnection = new Uri(settings.HostAddress!);
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(mqConnection);
            });
            // publish integration events
            x.AddRequestClient<IBasketCheckoutEvent>();
        });
    }
}
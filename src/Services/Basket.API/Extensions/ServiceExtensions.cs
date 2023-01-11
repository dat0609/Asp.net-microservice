using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Basket.API.Service;
using Basket.API.Service.Interface;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using Infrastructure.Common;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;
using Inventory.Grpc.Client;

namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
            .Get<EventBusSettings>();
        services.AddSingleton(eventBusSettings);
        
        var cacheSettings = configuration.GetSection(nameof(CacheSettings))
            .Get<CacheSettings>();
        services.AddSingleton(cacheSettings);
        
        var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
            .Get<GrpcSettings>();
        services.AddSingleton(grpcSettings);
        
        var backgroundJob = configuration.GetSection(nameof(BackgroundJobSettings))
            .Get<BackgroundJobSettings>();
        services.AddSingleton(backgroundJob);

        return services;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>()
            .AddTransient<IEmailTemplateService, BasketEmailTemplateService>();

    public static IServiceCollection ConfigureGrpcService(this IServiceCollection services)
    {
        var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
        services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(x => 
            x.Address = new Uri(settings.StockUrl));
        services.AddScoped<StockItemGrpcService>();
        return services;
    }

    public static void ConfigureRedis(this IServiceCollection services)
    {
        var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        if (string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("Redis Connection string is not configured.");
        
        //Redis Configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
        });
    }

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
        if (settings == null || string.IsNullOrEmpty(settings.HostAddress) ||
            string.IsNullOrEmpty(settings.HostAddress)) throw new ArgumentNullException("EventBusSettings is not configured!");

        var mqConnection = new Uri(settings.HostAddress);
        
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);
            });
            // Publish submit order message, instead of sending it to a specific queue directly.
            config.AddRequestClient<IBasketCheckoutEvent>();
        });
    }
    
    public static void ConfigureHttpClientService(this IServiceCollection services)
    {
        services.AddHttpClient<BackgroundJobHttpService>();
    }
}
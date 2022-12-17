using Inventory.Grpc.Repositories;
using Inventory.Grpc.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Grpc.Extension;

public static class ServiceExtension
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(MongoDbSettings))
            .Get<MongoDbSettings>();
        services.AddSingleton(databaseSettings);
        
        return services;
    }
    
    private static string GetMongoConnectionString(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(MongoDbSettings))
            .Get<MongoDbSettings>();
        if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("DatabaseSettings is not configured");

        var databaseName = settings.DatabaseName;
        var mongodbConnectionString = settings.ConnectionString + "/" + databaseName +
                                      "?authSource=admin";
        return mongodbConnectionString;
    }

    public static void ConfigureMongoDbClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(
                new MongoClient(GetMongoConnectionString(services, configuration)))
            .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
    }

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();
    }
}
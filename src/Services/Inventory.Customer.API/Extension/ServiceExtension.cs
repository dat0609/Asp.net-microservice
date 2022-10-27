using Infrastructure.Extensions;
using MongoDB.Driver;

namespace Inventory.Customer.API.Extension;

public static class ServiceExtension
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSetting = configuration.GetSection(nameof(MongoDbSettings))
            .Get<MongoDbSettings>();
        services.AddSingleton(databaseSetting);

        return services;
    }
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));
    }

    public static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(GetConnectionString(services)))
            .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
    }

    private static string GetConnectionString(this IServiceCollection services)
    {
        var settings = services.GetOption<MongoDbSettings>(nameof(MongoDbSettings));
        if (settings == null)
            throw new ArgumentNullException(nameof(MongoDbSettings));

        var databaseName = settings.DatabaseName;
        var mongoDbConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";
        
        return mongoDbConnectionString;
    }
}
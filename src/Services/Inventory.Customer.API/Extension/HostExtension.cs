using Inventory.Customer.API.Persistence;
using MongoDB.Driver;

namespace Inventory.Customer.API.Extension;

public static class HostExtension
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var settings = services.GetService<MongoDbSettings>();
        if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("DatabaseSettings is not configured");
        
        var mongoClient = services.GetRequiredService<IMongoClient>();
        new InventoryDbSeed()
            .SeedDataAsync(mongoClient, settings)
            .Wait();
        return host;
    }
}
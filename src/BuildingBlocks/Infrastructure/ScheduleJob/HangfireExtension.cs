using System.Security.Authentication;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.PostgreSql;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Shared.Configurations;

namespace Infrastructure.ScheduleJob;

public static class HangfireExtension
{
    public static IServiceCollection AddHangfireService(this IServiceCollection services)
    {
        var settings = services.GetOptions<HangfireSettings>("HangfireSettings");
        
        if (settings == null || settings.Storage == null ||
            string.IsNullOrEmpty(settings.Storage.ConnectionString))
            throw new Exception("HangFireSettings is not configured properly!");

        services.ConfigureHangfireServices(settings);
        services.AddHangfireServer(serverOptions 
            => { serverOptions.ServerName = settings.ServerName; });

        return services; 
    }
    
    private static IServiceCollection ConfigureHangfireServices(this IServiceCollection services, 
        HangfireSettings settings)
    {
        if (string.IsNullOrEmpty(settings.Storage.DBProvider))
            throw new Exception("HangFire DBProvider is not configured.");

        switch (settings.Storage.DBProvider.ToLower())
        {
            case "mongodb":
                var mongoUrlBuilder = new MongoUrlBuilder(settings.Storage.ConnectionString);

                var mongoClientSettings = MongoClientSettings.FromUrl(
                    new MongoUrl(settings.Storage.ConnectionString));
                mongoClientSettings.SslSettings = new SslSettings
                {
                    EnabledSslProtocols = SslProtocols.Tls12
                };
                var mongoClient = new MongoClient(mongoClientSettings);

                var mongoStorageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy(),
                    },
                    CheckConnection = true,
                    Prefix = "SchedulerQueue",
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                };
                
                services.AddHangfire((_, config) =>
                {
                    config.UseSimpleAssemblyNameTypeSerializer()
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseRecommendedSerializerSettings()
                        .UseConsole()
                        .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, mongoStorageOptions);

                    var jsonSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    config.UseSerializerSettings(jsonSettings);
                });
                
                services.AddHangfireConsoleExtensions();
                
                break;
            
            case "postgresql":
                services.AddHangfire(x =>
                    x.UsePostgreSqlStorage(settings.Storage.ConnectionString));
                break;
            
            case "mssql":
                break;
            
            default:
                throw new Exception( $"HangFire Storage Provider {settings.Storage.DBProvider} is not supported.");
        }

        return services;
    }
}
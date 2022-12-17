namespace OcelotApiGw.Extensions;

public static class HostExtension
{
    public static void AddAppConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddJsonFile($"ocelot.{env.EnvironmentName}.json", false, true)
                .AddEnvironmentVariables();
        });
    }
}
using Common.Logging;
using Inventory.Grpc.Extension;
using Inventory.Grpc.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Starting");

try
{
    // Additional configuration is required to successfully run gRPC on macOS.
    // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureMongoDbClient(builder.Configuration);
    builder.Services.AddInfrastructureServices();
    builder.Services.AddGrpc();

    var app = builder.Build();

    app.MapGrpcService<InventoryService>();

    // Configure the HTTP request pipeline.
    app.MapGet("/",
        () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
finally
{
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}
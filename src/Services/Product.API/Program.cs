using Common.Logging;
using Product.API.Context;
using Product.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Product API is starting up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    app.UseInfrastructure();

    app.MigrateDatabase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    }).Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Product API failed to start up");
}
finally
{
    Log.Information("Product API is shutting down");
    Log.CloseAndFlush();
}
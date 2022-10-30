using Basket.API;
using Basket.API.Extensions;
using Common.Logging;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Basket API is starting up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Services.ConfigureServices();
    builder.Services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.ConfigureGrpcServices(builder.Configuration);

    // config mass transit
    builder.Services.ConfigureMassTransit();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal("Basket API failed to start up");
}
finally
{
    Log.Information("Basket API is shutting down");
    Log.CloseAndFlush();
}
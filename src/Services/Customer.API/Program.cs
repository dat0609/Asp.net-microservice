using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Customer API is starting up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);

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

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal("Customer API failed to start up");
}
finally
{
    Log.Information("Customer API is shutting down");
    Log.CloseAndFlush();
}
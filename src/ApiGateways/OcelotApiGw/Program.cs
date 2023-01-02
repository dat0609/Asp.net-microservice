using Serilog;
using Common.Logging;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Information("API Gateway is starting up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);

    builder.Services.AddControllers();
    builder.Host.AddAppConfigurations();
    builder.Services.ConfigureOcelot(builder.Configuration);
    builder.Services.ConfigureCors();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        /*app.UseSwagger();
        app.UseSwaggerUI();*/
    }
    
    app.UseCors("CorsPolicy");
    app.UseAuthentication();
    app.UseRouting();
    //app.UseHttpsRedirection();

    app.UseAuthorization();
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", context =>
        {
             //context.Response.WriteAsync("Hello World!");
             context.Response.Redirect("swagger/index.html");
             return Task.CompletedTask;
        });
    });

    app.MapControllers();

    app.UseSwaggerForOcelotUI(x =>
    {
        x.PathToSwaggerGenerator = "/swagger/docs";
    });
    await app.UseOcelot();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "API Gateway failed to start");
    Log.Fatal("API Gateway failed to start up");
}
finally
{
    Log.Information("API Gateway is shutting down");
    Log.CloseAndFlush();
}
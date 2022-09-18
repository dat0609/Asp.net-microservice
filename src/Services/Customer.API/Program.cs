using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API;
using Customer.API.Context;
using Customer.API.Controllers;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
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
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(
        options => options.UseNpgsql(connectionString));

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped<ICustomerService, CustomerService>();

    var app = builder.Build();

    app.Map("/", () => "Minimum API");
    app.MapGet("api/customers",
        async (ICustomerService services) => await services.GetCustomersAsync());

    /*app.MapGet("api/customers/{username}",
        async (ICustomerService services, string username) => await services.GetCustomerByUsernameAsync(username));

    app.MapPost("api/customers",
        async (ICustomerService services, Customer.API.Entities.Customer customer) =>
            await services.CreateAsync(customer));*/
    app.MapCustomersAPI();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.SeedCustomerData()
        .Run();
}
catch (Exception)
{
    Log.Fatal("Customer API failed to start up");
}
finally
{
    Log.Information("Customer API is shutting down");
    Log.CloseAndFlush();
}
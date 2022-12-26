using System.Text;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Shared.Configurations;
using Shared.Identity;

namespace OcelotApiGw.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

        services.AddSingleton(jwt);
        
        return services;
    }
    
    public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration);
        services.AddTransient<ITokenService, TokenService>();
        services.AddJwtAuthentication();
    }
    
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }
    
    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var setting = services.GetOption<JwtSettings>(nameof(JwtSettings));
        if (setting == null)
        {
            throw new ArgumentNullException(nameof(setting));
        }
        
        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(setting.Key!));
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = false,
        };
        
        services.AddAuthentication(op =>
        {
            op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(op =>
        {
            op.RequireHttpsMetadata = false;
            op.SaveToken = true;
            op.TokenValidationParameters = tokenValidationParameters;
        });

        return services;
    }
}
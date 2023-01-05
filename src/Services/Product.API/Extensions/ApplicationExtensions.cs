using Infrastructure.Middlewares;

namespace Product.API.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API"));
        app.UseMiddleware<ErrorWrappingMiddleware>();
        // app.UseAuthentication();
        app.UseRouting();
        // app.UseHttpsRedirection(); //for production only
        // app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
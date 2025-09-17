using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WebApiCore.Configuration;

public static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection Services, OpenApiInfo infoApi)
    {
        Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(name: "v1", info: infoApi);
        });
    }

    public static void UseSwaggerConfiguration(this IApplicationBuilder app, bool IsDevelopment)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "");
            c.RoutePrefix = "swagger";
        });
        if (IsDevelopment)
        {
           
        }
    }
}

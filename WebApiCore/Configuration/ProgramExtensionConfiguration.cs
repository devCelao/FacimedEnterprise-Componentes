using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace WebApiCore.Configuration;

public static class ProgramExtensionConfiguration
{
    public static void AddExtensionConfiguration(this IServiceCollection Services)
    {
        Services.AddControllers()
       .AddJsonOptions(o =>
       {
           o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
           o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
       });
        
        Services.AddEndpointsApiExplorer();
    }

    public static void AddCustomCors(this IServiceCollection Services)
        =>
         Services.AddCors(options =>
         {
             options.AddPolicy("Total",
                 builder =>
                     builder
                         .AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader());
         });

    public static void UseCustomCors(this IApplicationBuilder app)
        =>
        app.UseCors("Total");
}
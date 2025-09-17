using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiCore.Extensions;

namespace WebApiCore.Configuration;

public static class APIExtensionConfiguration
{
    public static void AddHostsAPIConfiguration(this IServiceCollection Services, IConfiguration Configuration)
    {
        Services.Configure<ServicesHostSettingsModel>(Configuration);
    }
}

using MessageBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBus.Configuration;

public static class MessageBusDependencyInjection
{
    public static IServiceCollection AddMessageBusConfiguration(this IServiceCollection Services, string? connection)
    {
        if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException(nameof(connection));

        Services.AddSingleton<IMessageBus>(new MessageBus(connection));

        return Services;
    }
}

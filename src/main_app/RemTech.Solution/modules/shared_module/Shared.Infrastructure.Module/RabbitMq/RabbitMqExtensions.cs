using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Module.Consumers;

namespace Shared.Infrastructure.Module.RabbitMq;

public static class RabbitMqExtensions
{
    public static void AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqConnectionProvider>();
    }
}

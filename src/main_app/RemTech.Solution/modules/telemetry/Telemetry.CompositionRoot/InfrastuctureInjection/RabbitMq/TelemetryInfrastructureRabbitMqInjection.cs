using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;
using Telemetry.Infrastructure.RabbitMQ;

namespace Telemetry.CompositionRoot.InfrastuctureInjection.RabbitMq;

/// <summary>
/// Инъекция брокера сообщений у модуля телеметрии
/// </summary>
[InjectionClass]
public static class TelemetryInfrastructureRabbitMqInjection
{
    [InjectionMethod]
    public static void InjectTelemetryRabbitMqListeners(this IServiceCollection services)
    {
        services.AddHostedService<ActionInvokedEventListener>();
    }
}

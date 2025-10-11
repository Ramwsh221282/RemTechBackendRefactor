using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;

namespace Telemetry.CompositionRoot;

/// <summary>
/// инъекция сервиса модуля Телеметрии
/// </summary>
public static class TelemetryMethodInjection
{
    public static void InjectTelemetryModule(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies
    )
    {
        services.RegisterModuleServices(assemblies, "Telemetry");
    }
}

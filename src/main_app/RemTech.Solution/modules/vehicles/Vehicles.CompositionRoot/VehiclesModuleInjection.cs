using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;

namespace Vehicles.CompositionRoot;

public static class VehiclesModuleInjection
{
    public static void InjectVehiclesModule(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies
    )
    {
        services.RegisterModuleServices(assemblies, "Vehicles");
    }
}

using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;
using RemTech.UseCases.Shared.Cqrs;
using Vehicles.Domain.VehicleContext;
using Vehicles.UseCases.AddVehicle;

namespace Vehicles.CompositionRoot.UseCasesInjection.AddVehicle;

[InjectionClass]
public static class AddVehicleInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<AddVehicleCommand, Vehicle>, AddVehicleCommandHandler>();
    }
}

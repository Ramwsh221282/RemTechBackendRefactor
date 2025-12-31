using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Domain;

public static class VehiclesDomainInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterVehiclesDomain()
        {
            services.RegisterVehiclesDomainHandlers();
        }
        
        private void RegisterVehiclesDomainHandlers()
        {
            new HandlersRegistrator(services)
                .FromAssembly(typeof(VehiclesDomainInjection).Assembly)
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();
        }
    }
}
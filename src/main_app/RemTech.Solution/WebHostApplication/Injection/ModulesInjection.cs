using ContainedItems.Worker.Extensions;
using Identity.Domain.Accounts.Models;
using Identity.WebApi.Extensions;
using Notifications.Core.Mailers;
using Notifications.WebApi.Extensions;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.WebApi.Extensions;
using RemTech.SharedKernel.Core.Handlers;
using Spares.Domain.Models;
using Spares.WebApi.Extensions;
using Vehicles.Domain.Vehicles;
using Vehicles.WebApi.Extensions;

namespace WebHostApplication.Injection;

public static class ModulesInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterApplicationModules()
        {
            ParsersModuleInjection.AddInfrastructureLayer(services);
            NotificationsModuleInjection.AddInfrastructureLayer(services);
            IdentityModuleInjection.AddInfrastructure(services);
            ContainedItemsModuleInjection.RegisterInfrastructure(services);
            SparesModuleInjection.RegisterSparesInfrastructure(services);
            VehiclesModuleInjection.RegisterInfrastructureLayerDependencies(services);
            new HandlersRegistrator(services)
                .FromAssemblies(
                    [
                        typeof(Spare).Assembly, 
                        typeof(Account).Assembly, 
                        typeof(Mailer).Assembly, 
                        typeof(SubscribedParser).Assembly,
                        typeof(Vehicle).Assembly,
                        typeof(ContainedItemId).Assembly
                    ])
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .AlsoAddDomainEventHandlers()
                .AlsoAddDecorators()
                .AlsoAddValidators()
                .AlsoUseDecorators()
                .Invoke();


            // services.RegisterContainedItemsModule();
            // services.RegisterIdentityModule();
            // services.RegisterNotificationsModule();
            // services.RegisterParsersControlModule();
            // services.RegisterSparesModule();
            // services.RegisterVehiclesModule();
        }

        private void RegisterContainedItemsModule() =>
            services.InjectContainedItemsModule();

        private void RegisterIdentityModule() =>
            services.InjectIdentityModule();

        private void RegisterNotificationsModule() => 
            services.InjectNotificationsModule();

        private void RegisterParsersControlModule() =>
            services.InjectParsersControlModule();

        private void RegisterSparesModule() =>
            services.InjectSparesModule();

        private void RegisterVehiclesModule() =>
            services.InjectVehiclesModule();
    }
}
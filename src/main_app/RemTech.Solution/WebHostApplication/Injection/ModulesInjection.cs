using ContainedItems.Worker.Extensions;
using Identity.WebApi.Extensions;
using Notifications.WebApi.Extensions;
using ParsersControl.WebApi.Extensions;
using Spares.WebApi.Extensions;
using Vehicles.WebApi.Extensions;

namespace WebHostApplication.Injection;

public static class ModulesInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterApplicationModules()
        {
            services.RegisterContainedItemsModule();
            services.RegisterIdentityModule();
            services.RegisterNotificationsModule();
            services.RegisterParsersControlModule();
            services.RegisterSparesModule();
            services.RegisterVehiclesModule();
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
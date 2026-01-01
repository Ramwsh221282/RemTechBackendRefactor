using RemTech.Bootstrap.Api.Injection;
using RemTech.ContainedItems.Module.Injection;
using RemTech.Spares.Module.Injection;
using RemTech.Vehicles.Module.Injection;
using Users.Module.Injection;

namespace RemTech.Bootstrap.Api.Configuration;

public static class ModulesInject
{
    public static void InjectModules(this IServiceCollection services)
    {
        services.InjectCommonInfrastructure();
        services.InjectScrapersModule();
        services.InjectMailingModule();
        services.InjectUsersModule();
        services.InjectVehiclesModule();
        services.InjectSparesModule();
        services.InjectLocationsModule();
        services.InjectBrandsModule();
        services.InjectCategoriesModule();
        services.InjectModelsModule();
        services.InjectContainedItemsModule();
        services.InjectCleanersModule();
    }
}
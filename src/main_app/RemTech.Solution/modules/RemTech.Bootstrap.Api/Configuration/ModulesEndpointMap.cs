using Brands.Module.Injection;
using Categories.Module.Injection;
using Mailing.Module.Injection;
using RemTech.ContainedItems.Module.Injection;
using RemTech.Spares.Module.Injection;
using RemTech.Vehicles.Module.Injection;
using Scrapers.Module.Inject;
using Users.Module.Injection;

namespace RemTech.Bootstrap.Api.Configuration;

public static class ModulesEndpointMap
{
    public static void RegisterEndpoints(this WebApplication app)
    {
        app.MapMailingModuleEndpoints();
        app.MapVehiclesModuleEndpoints();
        app.MapUsersModuleEndpoints();
        app.MapScrapersModuleEndpoints();
        app.MapSparesEndpoints();
        app.MapContainedItemsModuleEndpoints();
        app.MapBrandsEndpoints();
        app.MapCategoriesEndpoints();
    }
}

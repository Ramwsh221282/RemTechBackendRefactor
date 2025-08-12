using Brands.Module.Injection;
using Categories.Module.Injection;
using GeoLocations.Module.Injection;
using Mailing.Module.Injection;
using Models.Module.Injection;
using RemTech.ContainedItems.Module.Injection;
using RemTech.Spares.Module.Injection;
using RemTech.Vehicles.Module.Injection;
using Scrapers.Module.Inject;
using Users.Module.Injection;

namespace RemTech.Bootstrap.Api.Configuration;

public static class DatabaseUp
{
    public static void UpDatabases(this RemTechApplicationSettings settings)
    {
        UsersModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        MailingModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        ParsedAdvertisementsInjection.UpDatabase(settings.Database.ToConnectionString());
        ScrapersModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        SparesModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        GeoLocationsModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        BrandsModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        CategoriesModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        ModelsModuleInjection.UpDatabase(settings.Database.ToConnectionString());
        ContainedItemsModuleInjection.UpDatabase(settings.Database.ToConnectionString());
    }
}

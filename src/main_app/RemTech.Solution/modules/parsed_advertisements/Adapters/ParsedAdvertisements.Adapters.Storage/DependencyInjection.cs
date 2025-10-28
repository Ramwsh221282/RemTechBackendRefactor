using Microsoft.Extensions.DependencyInjection;
using ParsedAdvertisements.Adapters.Storage.BrandContext;
using ParsedAdvertisements.Adapters.Storage.CategoryContext;
using ParsedAdvertisements.Adapters.Storage.CharacteristicContext;
using ParsedAdvertisements.Adapters.Storage.ModelContext;
using ParsedAdvertisements.Adapters.Storage.RegionContext;
using ParsedAdvertisements.Adapters.Storage.VehicleContext;
using ParsedAdvertisements.Domain.BrandContext.Ports;
using ParsedAdvertisements.Domain.CategoryContext.Ports;
using ParsedAdvertisements.Domain.CharacteristicContext.Ports;
using ParsedAdvertisements.Domain.ModelContext.Ports;
using ParsedAdvertisements.Domain.RegionContext.Ports;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Storage;

namespace ParsedAdvertisements.Adapters.Storage;

public static class DependencyInjection
{
    public static void AddParsedAdvertisementsStorageAdapter(this IServiceCollection services)
    {
        services.AddScoped<IBrandsStorage, BrandsStorage>();
        services.AddScoped<ICategoriesStorage, CategoriesStorage>();
        services.AddScoped<ICharacteristicsStorage, CharacteristicsStorage>();
        services.AddScoped<IModelsStorage, ModelsStorage>();
        services.AddScoped<IRegionsStorage, RegionsStorage>();
        services.AddScoped<IVehiclesStorage, VehiclesStorage>();
        services.AddScoped<ParsedAdvertisementsDbContext>();
    }
}
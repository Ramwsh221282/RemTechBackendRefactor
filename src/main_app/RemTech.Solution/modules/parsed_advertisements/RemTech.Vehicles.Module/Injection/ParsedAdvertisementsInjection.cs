using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Http;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Http;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Http;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Http;
using RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.BackgroundService;

namespace RemTech.Vehicles.Module.Injection;

public static class ParsedAdvertisementsInjection
{
    public static void InjectVehiclesModule(this IServiceCollection services)
    {
        services.AddHostedService<BackgroundJobTransportAdvertisementSinking>();
    }

    public static void UpVehiclesDatabase(this IServiceProvider provider)
    {
        DatabaseConfiguration config = provider.GetRequiredService<DatabaseConfiguration>();
        DatabaseBakery bakery = new(config);
        bakery.Up(typeof(ParsedAdvertisementsInjection).Assembly);
    }

    public static void MapVehiclesModuleEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/vehicles");
        group.CatalogueEndpoint();
        group.BrandsEndpoint();
        group.KindsEndpoint();
        group.ModelsEndpoint();
    }
}

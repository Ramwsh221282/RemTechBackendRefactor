using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Vehicles.Module.Features.QueryBrandsOfCategory;
using RemTech.Vehicles.Module.Features.QueryConcreteVehicle;
using RemTech.Vehicles.Module.Features.QueryModelsOfCategoryBrand;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands;
using RemTech.Vehicles.Module.Features.QueryVehicleCategories;
using RemTech.Vehicles.Module.Features.QueryVehicleModels;
using RemTech.Vehicles.Module.Features.QueryVehicleRegions;
using RemTech.Vehicles.Module.Features.QueryVehicles.Http;
using RemTech.Vehicles.Module.Features.QueryVehiclesAmount;
using RemTech.Vehicles.Module.Features.SimilarVehiclesQuery;
using RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.BackgroundService;

namespace RemTech.Vehicles.Module.Injection;

public static class ParsedAdvertisementsInjection
{
    public static void InjectVehiclesModule(this IServiceCollection services)
    {
        services.AddHostedService<BackgroundJobTransportAdvertisementSinking>();
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                typeof(BackgroundJobTransportAdvertisementSinking).Assembly
            )
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create parsers management database.");
    }
}

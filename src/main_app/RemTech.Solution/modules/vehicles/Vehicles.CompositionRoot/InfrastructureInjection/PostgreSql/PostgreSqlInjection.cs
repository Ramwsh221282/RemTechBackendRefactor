using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;
using RemTech.Infrastructure.PostgreSQL;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.VehicleContext.Infrastructure.DataSource;
using Vehicles.Infrastructure.PostgreSQL;
using Vehicles.Infrastructure.PostgreSQL.BrandContext;
using Vehicles.Infrastructure.PostgreSQL.CategoryContext;
using Vehicles.Infrastructure.PostgreSQL.LocationContext;
using Vehicles.Infrastructure.PostgreSQL.ModelContext;
using Vehicles.Infrastructure.PostgreSQL.VehicleContext;

namespace Vehicles.CompositionRoot.InfrastructureInjection.PostgreSql;

[InjectionClass]
public static class PostgreSqlInjection
{
    [InjectionMethod]
    public static void InjectInfrastructure(this IServiceCollection services)
    {
        services.InjectDatabaseDependencies<VehiclesServiceDbContext>();
        services.AddScoped<ILocationsDataSource, LocationsRepository>();
        services.AddScoped<ICategoryDataSource, CategoriesRepository>();
        services.AddScoped<IVehicleModelsDataSource, VehicleModelsRepository>();
        services.AddScoped<IBrandsDataSource, BrandsRepository>();
        services.AddScoped<IVehiclesDataSource, VehiclesRepository>();
    }
}

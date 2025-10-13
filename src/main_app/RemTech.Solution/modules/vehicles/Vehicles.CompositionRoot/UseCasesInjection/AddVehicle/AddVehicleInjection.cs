using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;
using RemTech.UseCases.Shared.Cqrs;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.Features.VehicleRegistration;
using Vehicles.Domain.VehicleContext.Infrastructure.DataSource;
using Vehicles.UseCases.AddVehicle;

namespace Vehicles.CompositionRoot.UseCasesInjection.AddVehicle;

[InjectionClass]
public static class AddVehicleInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddVehicleCommand>, AddVehicleCommandValidator>();
        services.AddScoped<ICommandHandler<AddVehicleCommand, Vehicle>, AddVehicleCommandHandler>();
        services.AddScoped<IVehicleRegistrator>(sp =>
        {
            ICategoryDataSource categories = sp.GetRequiredService<ICategoryDataSource>();
            IBrandsDataSource brands = sp.GetRequiredService<IBrandsDataSource>();
            IVehicleModelsDataSource models = sp.GetRequiredService<IVehicleModelsDataSource>();
            IVehiclesDataSource vehicles = sp.GetRequiredService<IVehiclesDataSource>();
            ILocationsDataSource locations = sp.GetRequiredService<ILocationsDataSource>();
            return new MovementVehicleRegistrator(
                new CreationalVehicleRegistrator(categories, brands, locations, models, vehicles)
            );
        });
    }
}

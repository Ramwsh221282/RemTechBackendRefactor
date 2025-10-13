using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;
using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Cqrs;
using RemTech.UseCases.Shared.DependencyInjection;
using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.ValueObjects;
using Vehicles.Infrastructure.PostgreSQL;
using Vehicles.Tests.Configurations;
using Vehicles.UseCases.AddVehicle;

namespace Vehicles.Tests.VehicleContext;

public sealed class VehicleTestsHelper
{
    private readonly IServiceProvider _services;

    public VehicleTestsHelper(VehiclesApplicationFactory factory)
    {
        _services = factory.Services;
    }

    public async Task<Result<Vehicle>> AddVehicle(
        string categoryName,
        string brandName,
        string modelName,
        IEnumerable<string> locationParts,
        string description,
        long price,
        bool isNds,
        IEnumerable<AddVehicleCommandCharacteristic> characteristics,
        IEnumerable<string> photoPaths
    )
    {
        AddVehicleCommand command = new AddVehicleCommand(
            categoryName,
            brandName,
            modelName,
            locationParts,
            description,
            new AddVehicleCommandPriceInfo(price, isNds),
            characteristics,
            photoPaths
        );
        return await AddVehicle(command);
    }

    public async Task<Result<Vehicle>> GetVehicleById(VehicleId vehicleId)
    {
        await using AsyncServiceScope scope = _services.CreateAsyncScope();
        await using VehiclesServiceDbContext context = scope.GetService<VehiclesServiceDbContext>();
        Vehicle? vehicle = await context
            .Vehicles.Include(v => v.Brand)
            .Include(v => v.Model)
            .Include(v => v.Location)
            .Include(v => v.Category)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);
        return vehicle == null
            ? Error.NotFound($"Техника с ID: {vehicleId.Value} не найдена.")
            : vehicle;
    }

    public async Task<Result<Vehicle>> AddVehicle(AddVehicleCommand command)
    {
        await using var scope = _services.CreateAsyncScope();
        var handler = scope.GetCommandHandler<AddVehicleCommand, Vehicle>();
        return await handler.Handle(command);
    }
}

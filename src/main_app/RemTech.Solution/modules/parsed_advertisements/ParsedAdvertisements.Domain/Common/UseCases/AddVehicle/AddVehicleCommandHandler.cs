using ParsedAdvertisements.Domain.BrandContext.Ports;
using ParsedAdvertisements.Domain.CategoryContext.Ports;
using ParsedAdvertisements.Domain.CharacteristicContext.Ports;
using ParsedAdvertisements.Domain.CharacteristicContext.ValueObjects;
using ParsedAdvertisements.Domain.ModelContext.Ports;
using ParsedAdvertisements.Domain.RegionContext.Ports;
using ParsedAdvertisements.Domain.VehicleContext;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Storage;
using ParsedAdvertisements.Domain.VehicleContext.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.Common.UseCases.AddVehicle;

public sealed class AddVehicleCommandHandler(
    ITransactionManager txn,
    IBrandsStorage brands,
    ICategoriesStorage categories,
    IRegionsStorage regions,
    IModelsStorage models,
    ICharacteristicsStorage characteristics,
    IVehiclesStorage vehicles)
    : ICommandHandler<AddVehicleCommand, Status<Vehicle>>
{
    public async Task<Status<Vehicle>> Handle(AddVehicleCommand command, CancellationToken ct = default)
    {
        var brand = await brands.Get(command.BrandName, txn, ct);
        if (brand.IsFailure)
            return brand.Error;

        var category = await categories.Get(command.CategoryName, txn, ct);
        if (category.IsFailure)
            return category.Error;

        var region = await regions.Get(command.Region, txn, ct);
        if (region.IsFailure)
            return region.Error;

        var model = await models.Get(command.ModelName, txn, ct);
        if (model.IsFailure)
            return model.Error;

        var vehicleIdentity = new VehicleIdentitySpecification(command.VehicleId)
            .OfBrand(brand)
            .OfCategory(category)
            .OfModel(model)
            .OfLocation(region);

        if (!vehicleIdentity.IsValid(out var error))
            return error;

        var vehicleCharacteristics = await CreateCharacteristicsList(command, vehicleIdentity, ct);

        var photos = UniquePhotoColleciton.Create(command.Photos);
        var vehiclePrice = VehiclePriceSpecification.Create(command.Price, command.IsNds);
        var source = VehicleSourceSpecification.Create(command.SourceUrl, command.SourceDomain);
        var path = VehicleLocationPath.Create(command.Locations);

        var vehicle = new Vehicle(
            vehicleIdentity,
            vehiclePrice,
            source,
            vehicleCharacteristics,
            photos.Value.ToVehiclePhotosList(), path);

        await vehicles.Save(vehicle, txn, ct);
        return vehicle;
    }

    private async Task<Status<VehicleCharacteristicsList>> CreateCharacteristicsList(
        AddVehicleCommand command,
        VehicleIdentitySpecification identity,
        CancellationToken ct = default)
    {
        var characteristicsToMatch = command.Characteristics
            .Select(c => new CharacteristicSimilarity(CharacteristicName.Create(c.Name), c.Value));

        var matched = await characteristics.GetSimilar(characteristicsToMatch, txn, ct);
        var vehicleCharacteristics = new VehicleCharacteristicsList();

        foreach (var ctx in matched)
        {
            var entry = ctx.ToVehicleCharacteristic(identity);
            if (entry.IsFailure)
                return entry.Error;

            var appending = vehicleCharacteristics.Add(entry);
            if (appending.IsFailure)
                return appending.Error;
        }

        return vehicleCharacteristics;
    }
}
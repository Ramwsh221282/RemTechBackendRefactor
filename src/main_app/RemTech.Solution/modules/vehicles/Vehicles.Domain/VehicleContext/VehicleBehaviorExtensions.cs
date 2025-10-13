using RemTech.Core.Shared.Enumerables;
using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.VehicleContext.Errors;
using Vehicles.Domain.VehicleContext.Infrastructure.DataSource;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext;

public static class VehicleBehaviorExtensions
{
    public static async Task<Result<Vehicle>> Save(
        this Vehicle vehicle,
        IVehiclesDataSource vehicles,
        IVehicleModelsDataSource models,
        ICategoryDataSource categories,
        IBrandsDataSource brands,
        ILocationsDataSource locations,
        CancellationToken ct = default
    ) =>
        await vehicle
            .Validate()
            .Match(
                onSuccess: async success =>
                {
                    Vehicle succedeed = success.Value;
                    succedeed.Brand = await brands.GetOrSave(succedeed.Brand, ct);
                    succedeed.Brand = succedeed.Brand.AddVehicle();
                    succedeed.Location = await locations.GetOrSave(succedeed.Location, ct);
                    succedeed.Location = succedeed.Location.AddVehicle();
                    succedeed.Model = await models.GetOrSave(succedeed.Model, ct);
                    succedeed.Model = succedeed.Model.AddVehicle();
                    succedeed.Category = await categories.GetOrSave(succedeed.Category, ct);
                    succedeed.Category = succedeed.Category.AddVehicle();
                    return await vehicles.Add(vehicle, ct);
                },
                onFailure: result => result.Error
            );

    public static Result<Vehicle> Validate(this Vehicle vehicle) =>
        vehicle switch
        {
            { Id: VehicleId id } when id.Validate().IsFailure => id.Validate().Error,
            { Description: VehicleDescription desc } when desc.Validate().IsFailure =>
                desc.Validate().Error,
            { Characteristics: VehicleCharacteristicsCollection ctx }
                when ctx.Validate().IsFailure => ctx.Validate().Error,
            { Photos: VehiclePhotosCollection photos } when photos.Validate().IsFailure => photos
                .Validate()
                .Error,
            { Brand: Brand brand } when brand.Validate().IsFailure => brand.Validate().Error,
            { Category: Category category } when category.Validate().IsFailure => category
                .Validate()
                .Error,
            { Location: Location location } when location.Validate().IsFailure => location
                .Validate()
                .Error,
            { Model: VehicleModel model } when model.Validate().IsFailure => model.Validate().Error,
            _ => vehicle,
        };

    public static Result<VehicleId> Validate(this VehicleId vehicleId) =>
        vehicleId.Value switch
        {
            _ when vehicleId.Value == Guid.Empty => new VehicleIdEmptyError(),
            _ => vehicleId,
        };

    public static Result<VehicleDescription> Validate(this VehicleDescription description) =>
        description.Value switch
        {
            _ when string.IsNullOrWhiteSpace(description.Value) =>
                new VehicleDescriptionEmptyError(),
            _ when description.Value.Length > VehicleDescription.MaxLength =>
                new VehicleDescriptionExceesLengthError(VehicleDescription.MaxLength),
            _ => description,
        };

    public static Result<VehicleCharacteristicsCollection> Validate(
        this VehicleCharacteristicsCollection characteristics
    ) =>
        characteristics.Characteristics switch
        {
            [] => new VehicleCharacteristicsCollectionEmptyError(),
            _ when characteristics.Characteristics.HasRepeatableValues(
                    c => c.Name,
                    out VehicleCharacteristic[] repeatable
                ) => Error.Validation(
                repeatable.CreateRepeatableValuesMessage(
                    c => c.Name,
                    "Характеристики техники повторяются: "
                )
            ),
            _ when characteristics.Characteristics.MatchTo(
                    c => c.Validate(),
                    result => result.IsFailure,
                    out Result<VehicleCharacteristic> failed
                ) => failed.Error,
            _ => characteristics,
        };

    public static Result<VehicleCharacteristic> Validate(
        this VehicleCharacteristic characteristic
    ) =>
        characteristic switch
        {
            { Value: null } => new VehicleCharacteristicValueEmptyError(),
            { Name: null } => new VehicleCharacteristicNameEmptyError(),
            { Value: string value } when string.IsNullOrWhiteSpace(value) =>
                new VehicleCharacteristicValueEmptyError(),
            { Name: string name } when string.IsNullOrWhiteSpace(name) =>
                new VehicleCharacteristicNameEmptyError(),
            { Name: { Length: > VehicleCharacteristic.MaxLength } } =>
                new VehicleCharacteristicNameExceesLengthError(VehicleCharacteristic.MaxLength),
            { Value: { Length: > VehicleCharacteristic.MaxLength } } =>
                new VehicleCharacteristicValueExceesLengthError(VehicleCharacteristic.MaxLength),
            _ => characteristic,
        };

    public static Result<VehiclePhoto> Validate(this VehiclePhoto photo) =>
        photo.Path switch
        {
            null => new VehiclePhotoPathEmptyError(),
            not null when string.IsNullOrWhiteSpace(photo.Path) => new VehiclePhotoPathEmptyError(),
            _ => photo,
        };

    public static Result<VehiclePhotosCollection> Validate(this VehiclePhotosCollection photos) =>
        photos.Photos switch
        {
            [] => new VehiclePhotosCollectionEmptyError(),
            _ when photos.Photos.HasRepeatableValues(p => p, out VehiclePhoto[] repeatable) =>
                Error.Validation(
                    repeatable.CreateRepeatableValuesMessage(
                        p => p.Path,
                        "Список фотографий содержит дубликаты: "
                    )
                ),
            _ when photos.Photos.MatchTo(
                    c => c.Validate(),
                    result => result.IsFailure,
                    out Result<VehiclePhoto> failed
                ) => failed.Error,
            _ => photos,
        };
}

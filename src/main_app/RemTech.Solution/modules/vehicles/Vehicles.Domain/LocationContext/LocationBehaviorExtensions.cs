using RemTech.Core.Shared.Enumerables;
using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Errors;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.VehicleContext;

namespace Vehicles.Domain.LocationContext;

public static class LocationBehaviorExtensions
{
    public static async Task<Result<Location>> Save(
        this Location location,
        ILocationsDataSource dataSource,
        CancellationToken ct = default
    ) =>
        await location
            .Validate()
            .Match(
                onSuccess: async success => await dataSource.GetOrSave(success.Value, ct),
                onFailure: failure => failure.Error
            );

    public static Location AddVehicle(this Location location)
    {
        location.VehicleCount = location.VehicleCount.Increase();
        return location;
    }

    public static LocationVehiclesCount Increase(
        this LocationVehiclesCount locationVehiclesCount
    ) => new(locationVehiclesCount.Value + 1);

    public static Result<Location> Validate(this Location location) =>
        location switch
        {
            { Id: LocationId id } when id.Validate().IsFailure => id.Validate().Error,
            { Rating: LocationRating rating } when rating.Validate().IsFailure => rating
                .Validate()
                .Error,
            { Address: LocationAddress address } when address.Validate().IsFailure => address
                .Validate()
                .Error,
            { VehicleCount: LocationVehiclesCount count } when count.Validate().IsFailure => count
                .Validate()
                .Error,
            _ => location,
        };

    public static Result<LocationId> Validate(this LocationId id) =>
        id switch
        {
            { Value: Guid value } when value == Guid.Empty => new LocationIdEmptyError(),
            _ => id,
        };

    public static Result<LocationAddress> Validate(this LocationAddress address) =>
        address.Parts switch
        {
            [] => new LocationAddressPartsEmptyError(),
            _ when address.Parts.HasRepeatableValues(
                    p => p.Value,
                    out LocationAddressPart[] repeatable
                ) => Error.Validation(
                repeatable.CreateRepeatableValuesMessage(
                    r => r.Value,
                    "Адрес техники состоит из повторных частей: "
                )
            ),
            _ when address.Parts.MatchTo(
                    p => p.Validate(),
                    res => res.IsFailure,
                    out Result<LocationAddressPart> failued
                ) => failued.Error,
            _ => address,
        };

    public static Result<LocationAddressPart> Validate(this LocationAddressPart part) =>
        part switch
        {
            { Value: string value } when string.IsNullOrWhiteSpace(value) =>
                new LocationAddressPartEmptyError(),
            { Value: { Length: > LocationAddressPart.MaxLength } } =>
                new LocationAddressPartExceesLengthError(LocationAddressPart.MaxLength),
            _ => part,
        };

    public static Result<LocationRating> Validate(this LocationRating rating) =>
        rating.Value < 0 ? new LocationRatingNegativeError() : rating;

    public static Result<LocationVehiclesCount> Validate(
        this LocationVehiclesCount vehiclesCount
    ) => vehiclesCount.Value < 0 ? new LocationVehiclesCountNegativeError() : vehiclesCount;
}

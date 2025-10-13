using RemTech.Core.Shared.Extensions;
using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.Errors;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.VehicleContext;

namespace Vehicles.Domain.BrandContext;

public static class BrandBehaviorExtensions
{
    public static async Task<Result<Brand>> Save(
        this Brand brand,
        IBrandsDataSource dataSource,
        CancellationToken ct = default
    ) =>
        await brand
            .Validate()
            .Match(
                onSuccess: async success =>
                {
                    Brand saved = await dataSource.GetOrSave(success.Value, ct);
                    return saved;
                },
                onFailure: failure => failure.Error
            );

    public static Result<Brand> Validate(this Brand brand) =>
        brand switch
        {
            { Name: BrandName name } when name.Validate().IsFailure => name.Validate().Error,
            { Id: BrandId id } when id.Validate().IsFailure => id.Validate().Error,
            { VehiclesCount: BrandVehiclesCount count } when count.Validate().IsFailure => count
                .Validate()
                .Error,
            { Rating: BrandRating rating } when rating.Validate().IsFailure => rating
                .Validate()
                .Error,
            _ => brand,
        };

    public static Brand AddVehicle(this Brand brand)
    {
        brand.VehiclesCount = brand.VehiclesCount.Increase();
        return brand;
    }

    public static BrandVehiclesCount Increase(this BrandVehiclesCount count) =>
        new(count.Value + 1);

    public static Result<BrandId> Validate(this BrandId id) =>
        id.Id == Guid.Empty ? new BrandIdEmptyError() : id;

    public static Result<BrandRating> Validate(this BrandRating rating) =>
        rating.Value < 0 ? new BrandNegativeRatingError() : rating;

    public static Result<BrandVehiclesCount> Validate(this BrandVehiclesCount count) =>
        count.Value < 0 ? new BrandNegativeVehiclesCountError() : count;

    public static Result<BrandName> Validate(this BrandName name) =>
        name switch
        {
            { Name: string value } when value.IsNullOrWhiteSpace() => new BrandNameEmptyError(),
            { Name: { Length: > BrandName.MaxLength } } => new BrandNameExceesLengthError(
                BrandName.MaxLength
            ),
            _ => name,
        };
}

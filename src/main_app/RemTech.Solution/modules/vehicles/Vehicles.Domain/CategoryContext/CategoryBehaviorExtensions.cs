using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.Errors;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.VehicleContext;

namespace Vehicles.Domain.CategoryContext;

public static class CategoryBehaviorExtensions
{
    public static Category AddVehicle(this Category category)
    {
        category.VehiclesCount = category.VehiclesCount.Increase();
        return category;
    }

    public static CategoryVehiclesCount Increase(this CategoryVehiclesCount count) =>
        new(count.Value + 1);

    public static async Task<Result<Category>> Save(
        this Category category,
        ICategoryDataSource dataSource,
        CancellationToken ct = default
    ) =>
        await category
            .Validate()
            .Match(
                onSuccess: async success =>
                {
                    Category saved = await dataSource.GetOrSave(success.Value, ct);
                    return saved;
                },
                onFailure: failure => failure.Error
            );

    public static Result<Category> Validate(this Category category) =>
        category switch
        {
            { Id: CategoryId id } when id.Validate().IsFailure => id.Validate().Error,
            { Name: CategoryName name } when Validate(name).IsFailure => Validate(name).Error,
            { Rating: CategoryRating rating } when rating.Validate().IsFailure => rating
                .Validate()
                .Error,
            { VehiclesCount: CategoryVehiclesCount count } when count.Validate().IsFailure => count
                .Validate()
                .Error,
            _ => category,
        };

    public static Result<CategoryId> Validate(this CategoryId id) =>
        id switch
        {
            { Value: Guid value } when value == Guid.Empty => new CategoryIdEmptyError(),
            _ => id,
        };

    public static Result<CategoryName> Validate(this CategoryName name) =>
        name switch
        {
            { Value: string value } when string.IsNullOrWhiteSpace(value) =>
                new CategoryNameEmptyError(),
            { Value: { Length: > CategoryName.MaxLength } } => new CategoryNameExceesLengthError(
                CategoryName.MaxLength
            ),
            _ => name,
        };

    public static Result<CategoryRating> Validate(this CategoryRating rating) =>
        rating.Value < 0 ? new CategoryRatingNegativeError() : rating;

    public static Result<CategoryVehiclesCount> Validate(
        this CategoryVehiclesCount vehiclesCount
    ) => vehiclesCount.Value < 0 ? new CategoryOwnedVehiclesCountNegativeError() : vehiclesCount;
}

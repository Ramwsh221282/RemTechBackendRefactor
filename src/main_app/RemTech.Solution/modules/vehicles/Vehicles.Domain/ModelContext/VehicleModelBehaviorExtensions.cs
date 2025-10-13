using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.Errors;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext;

namespace Vehicles.Domain.ModelContext;

public static class VehicleModelBehaviorExtensions
{
    public static async Task<Result<VehicleModel>> Save(
        this VehicleModel model,
        IVehicleModelsDataSource dataSource,
        CancellationToken ct = default
    ) =>
        await model
            .Validate()
            .Match(
                onSuccess: async success => await dataSource.GetOrSave(success.Value, ct),
                onFailure: failure => failure.Error
            );

    public static VehicleModel AddVehicle(this VehicleModel model)
    {
        model.VehiclesCount = model.VehiclesCount.Increase();
        return model;
    }

    public static VehicleModelVehicleCount Increase(this VehicleModelVehicleCount count) =>
        new(count.Value + 1);

    public static Result<VehicleModel> Validate(this VehicleModel model) =>
        model switch
        {
            { Name: VehicleModelName name } when name.Validate().IsFailure => name.Validate().Error,
            { Id: VehicleModelId id } when id.Validate().IsFailure => id.Validate().Error,
            { Rating: VehicleModelRating rating } when rating.Validate().IsFailure => rating
                .Validate()
                .Error,
            { VehiclesCount: VehicleModelVehicleCount count } when count.Validate().IsFailure =>
                count.Validate().Error,
            _ => model,
        };

    public static Result<VehicleModelId> Validate(this VehicleModelId id) =>
        id switch
        {
            { Value: Guid value } when value == Guid.Empty => new VehicleModelIdEmptyError(),
            _ => id,
        };

    public static Result<VehicleModelName> Validate(this VehicleModelName name) =>
        name switch
        {
            { Value: string value } when string.IsNullOrWhiteSpace(value) =>
                new VehicleModelNameEmptyError(),
            { Value: { Length: > VehicleModelName.MaxLength } } =>
                new VehicleModelNameExceesLengthError(VehicleModelName.MaxLength),
            _ => name,
        };

    public static Result<VehicleModelRating> Validate(this VehicleModelRating rating) =>
        rating switch
        {
            { Value: long and < 0 } => new VehicleModelRatingNegativeError(),
            _ => rating,
        };

    public static Result<VehicleModelVehicleCount> Validate(this VehicleModelVehicleCount count) =>
        count switch
        {
            { Value: long and < 0 } => new VehicleModelVehicleCountNegativeError(),
            _ => count,
        };
}

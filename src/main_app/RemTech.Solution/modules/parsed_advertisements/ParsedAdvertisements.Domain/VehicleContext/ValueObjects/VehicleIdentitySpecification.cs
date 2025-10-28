using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed record VehicleIdentitySpecification
{
    public VehicleIdentitySpecification(string vehicleId) =>
        (VehicleId, CategoryId, BrandId, LocationId, ModelId) = (
            vehicleId,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty
        );

    private VehicleIdentitySpecification(
        string vehicleId,
        Guid categoryId,
        Guid brandId,
        Guid locationId,
        Guid modelId
    ) =>
        (VehicleId, CategoryId, BrandId, LocationId, ModelId) = (
            vehicleId,
            categoryId,
            brandId,
            locationId,
            modelId
        );

    public string VehicleId { get; } = string.Empty;
    public Guid CategoryId { get; } = Guid.Empty;
    public Guid BrandId { get; } = Guid.Empty;
    public Guid LocationId { get; } = Guid.Empty;
    public Guid ModelId { get; } = Guid.Empty;

    public VehicleIdentitySpecification OfCategory(Guid categoryId) =>
        new(VehicleId, CategoryId, BrandId, LocationId, ModelId);

    public VehicleIdentitySpecification OfBrand(Guid brandId) =>
        new(VehicleId, CategoryId, BrandId, LocationId, ModelId);

    public VehicleIdentitySpecification OfLocation(Guid locationId) =>
        new(VehicleId, CategoryId, BrandId, LocationId, ModelId);

    public VehicleIdentitySpecification OfModel(Guid modelId) =>
        new(VehicleId, CategoryId, BrandId, LocationId, ModelId);

    public bool IsValid(out Error error)
    {
        List<Error> errors = [];

        ValidateVehicleId(errors);
        ValidateCategoryId(errors);
        ValidateBrandId(errors);
        ValidateLocationId(errors);
        ValidateModelId(errors);

        error = errors.Count == 0 ? Error.None() : Error.Combined(errors);
        return !error.Any();
    }

    private void ValidateVehicleId(List<Error> errors)
    {
        if (string.IsNullOrWhiteSpace(VehicleId))
            errors.Add(Error.Validation("Идентификатор техники не может быть пустым."));
    }

    private void ValidateCategoryId(List<Error> errors)
    {
        if (CategoryId == Guid.Empty)
            errors.Add(Error.Validation("Идентификатор категории техники не может быть пустым."));
    }

    private void ValidateBrandId(List<Error> errors)
    {
        if (BrandId == Guid.Empty)
            errors.Add(Error.Validation("Идентификатор марки техники не может быть пустым."));
    }

    private void ValidateLocationId(List<Error> errors)
    {
        if (LocationId == Guid.Empty)
            errors.Add(Error.Validation("Идентификатор локации техники не может быть пустым."));
    }

    private void ValidateModelId(List<Error> errors)
    {
        if (ModelId == Guid.Empty)
            errors.Add(Error.Validation("Идентификатор модели техники не может быть пустым."));
    }
}

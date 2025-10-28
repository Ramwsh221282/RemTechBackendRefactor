using ParsedAdvertisements.Domain.BrandContext;
using ParsedAdvertisements.Domain.BrandContext.ValueObjects;
using ParsedAdvertisements.Domain.CategoryContext;
using ParsedAdvertisements.Domain.CategoryContext.ValueObjects;
using ParsedAdvertisements.Domain.ModelContext;
using ParsedAdvertisements.Domain.ModelContext.ValueObjects;
using ParsedAdvertisements.Domain.RegionContext;
using ParsedAdvertisements.Domain.RegionContext.ValueObjects;
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
        new(VehicleId, categoryId, BrandId, LocationId, ModelId);

    public VehicleIdentitySpecification OfBrand(Guid brandId) =>
        new(VehicleId, CategoryId, brandId, LocationId, ModelId);

    public VehicleIdentitySpecification OfLocation(Guid locationId) =>
        new(VehicleId, CategoryId, BrandId, locationId, ModelId);

    public VehicleIdentitySpecification OfModel(Guid modelId) =>
        new(VehicleId, CategoryId, BrandId, LocationId, modelId);

    public VehicleIdentitySpecification OfCategory(Category category) => OfCategory(category.Id);

    public VehicleIdentitySpecification OfBrand(Brand brand) => OfBrand(brand.Id);

    public VehicleIdentitySpecification OfLocation(Region region) => OfLocation(region.Id);

    public VehicleIdentitySpecification OfModel(Model model) => OfModel(model.Id);

    public VehicleIdentitySpecification OfCategory(CategoryId id) => OfCategory(id.Id);

    public VehicleIdentitySpecification OfBrand(BrandId id) => OfBrand(id.Id);

    public VehicleIdentitySpecification OfLocation(RegionId id) => OfLocation(id.Value);

    public VehicleIdentitySpecification OfModel(ModelId id) => OfModel(id.Value);

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
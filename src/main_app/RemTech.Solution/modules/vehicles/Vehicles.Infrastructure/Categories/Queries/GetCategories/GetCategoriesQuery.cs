using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

public class GetCategoriesQuery : IQuery
{
    public Guid? BrandId { get; private init; }
    public string? BrandName { get; private init; }
    public Guid? ModelId { get; private init; }
    public string? ModelName { get; private init; }
    public Guid? Id { get; private init; }
    public string? Name { get; private init; }

    public GetCategoriesQuery ForId(Guid? id) =>
        id == null || id == Guid.Empty ? this : Copy(this, id: id);

    public GetCategoriesQuery ForName(string? name) =>
        string.IsNullOrWhiteSpace(name) ? this : Copy(this, name: name);

    public GetCategoriesQuery ForBrandId(Guid? brandId) =>
        brandId == null || brandId == Guid.Empty ? this : Copy(this, brandId: brandId);

    public GetCategoriesQuery ForBrandName(string? brandName) =>
        string.IsNullOrWhiteSpace(brandName) ? this : Copy(this, brandName: brandName);

    public GetCategoriesQuery ForModelId(Guid? modelId) =>
        modelId == null || modelId == Guid.Empty ? this : Copy(this, modelId: modelId);

    public GetCategoriesQuery ForModelName(string? modelName) =>
        string.IsNullOrWhiteSpace(modelName) ? this : Copy(this, modelName: modelName);

    public override string ToString()
    {
        return string.Empty;
    }

    private static GetCategoriesQuery Copy(
        GetCategoriesQuery origin,
        Guid? brandId = null,
        string? brandName = null,
        Guid? modelId = null,
        string? modelName = null,
        Guid? id = null,
        string? name = null
    ) =>
        new()
        {
            BrandId = brandId ?? origin.BrandId,
            BrandName = brandName ?? origin.BrandName,
            ModelId = modelId ?? origin.ModelId,
            ModelName = modelName ?? origin.ModelName,
            Id = id ?? origin.Id,
            Name = name ?? origin.Name,
        };
}

using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrand;

public sealed record GetBrandQuery : IQuery
{
    public Guid? CategoryId { get; private set; }
    public string? CategoryName { get; private set; }
    public string? BrandName { get; private set; }
    public Guid? BrandId { get; private set; }
    public Guid? ModelId { get; private set; }
    public string? ModelName { get; private set; }

    public bool HasNoInitializedProperties() =>
        CategoryId == null
        && string.IsNullOrWhiteSpace(CategoryName)
        && BrandId == null
        && string.IsNullOrWhiteSpace(BrandName)
        && ModelId == null
        && string.IsNullOrWhiteSpace(ModelName);

    public GetBrandQuery ForCategoryId(Guid? categoryId)
    {
        if (categoryId == null || categoryId.Value == Guid.Empty || CategoryId != null)
            return this;
        CategoryId = categoryId;
        return this;
    }

    public GetBrandQuery ForCategoryName(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName) || !string.IsNullOrWhiteSpace(CategoryName))
            return this;
        CategoryName = categoryName;
        return this;
    }

    public GetBrandQuery ForBrandId(Guid? brandId)
    {
        if (brandId == null || brandId.Value == Guid.Empty || BrandId != null)
            return this;
        BrandId = brandId;
        return this;
    }

    public GetBrandQuery ForBrandName(string? brandName)
    {
        if (string.IsNullOrWhiteSpace(brandName) || !string.IsNullOrWhiteSpace(BrandName))
            return this;
        BrandName = brandName;
        return this;
    }

    public GetBrandQuery ForModelId(Guid? modelId)
    {
        if (modelId == null || modelId.Value == Guid.Empty || ModelId != null)
            return this;
        ModelId = modelId;
        return this;
    }

    public GetBrandQuery ForModelName(string? modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName) || !string.IsNullOrWhiteSpace(ModelName))
            return this;
        ModelName = modelName;
        return this;
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}

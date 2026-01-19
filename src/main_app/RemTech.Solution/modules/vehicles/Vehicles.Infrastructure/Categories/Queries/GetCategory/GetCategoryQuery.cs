using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Categories.Queries.GetCategory;

public sealed class GetCategoryQuery : IQuery
{
    public Guid? CategoryId { get; private set; }
    public string? CategoryName { get; private set; }
    public Guid? BrandId { get; private set; }
    public string? BrandName { get; private set; }
    public Guid? ModelId { get; private set; }
    public string? ModelName { get; private set; }

    public GetCategoryQuery ForModelId(Guid? modelId)
    {
        if (modelId == null || modelId.Value == Guid.Empty || ModelId != null)
            return this;
        ModelId = modelId;
        return this;
    }

    public GetCategoryQuery ForModelName(string? modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName) || !string.IsNullOrWhiteSpace(ModelName))
            return this;
        ModelName = modelName;
        return this;
    }

    public GetCategoryQuery ForId(Guid? id)
    {
        if (id == null || id.Value == Guid.Empty || CategoryId != null)
            return this;
        CategoryId = id.Value;
        return this;
    }

    public GetCategoryQuery ForName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(CategoryName))
            return this;
        CategoryName = name;
        return this;
    }

    public GetCategoryQuery ForBrandId(Guid? brandId)
    {
        if (brandId == null || brandId.Value == Guid.Empty || BrandId != null)
            return this;
        BrandId = brandId;
        return this;
    }

    public GetCategoryQuery ForBrandName(string? brandName)
    {
        if (string.IsNullOrWhiteSpace(brandName) || !string.IsNullOrWhiteSpace(BrandName))
            return this;
        BrandName = brandName;
        return this;
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}

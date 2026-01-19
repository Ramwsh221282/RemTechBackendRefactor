using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;
using Vehicles.Domain.Models;

namespace Vehicles.Infrastructure.Models.Queries.GetModel;

public sealed class GetModelQuery : IQuery
{
    public Guid? Id { get; private set; }
    public string? Name { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string? CategoryName { get; private set; }
    public Guid? BrandId { get; private set; }
    public string? BrandName { get; private set; }

    public bool HasNoInitializedProperties() =>
        Id == null
        && string.IsNullOrWhiteSpace(Name)
        && CategoryId == null
        && string.IsNullOrWhiteSpace(CategoryName)
        && BrandId == null
        && string.IsNullOrWhiteSpace(BrandName);

    public GetModelQuery ForId(Guid? id)
    {
        if (id == null || id.Value == Guid.Empty || Id != null)
            return this;
        Id = id;
        return this;
    }

    public GetModelQuery ForName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(Name))
            return this;
        Name = name;
        return this;
    }

    public GetModelQuery ForCategoryId(Guid? categoryId)
    {
        if (categoryId == null || categoryId.Value == Guid.Empty || CategoryId != null)
            return this;
        CategoryId = categoryId;
        return this;
    }

    public GetModelQuery ForCategoryName(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName) || !string.IsNullOrWhiteSpace(CategoryName))
            return this;
        CategoryName = categoryName;
        return this;
    }

    public GetModelQuery ForBrandId(Guid? brandId)
    {
        if (brandId == null || brandId.Value == Guid.Empty || BrandId != null)
            return this;
        BrandId = brandId;
        return this;
    }

    public GetModelQuery ForBrandName(string? brandName)
    {
        if (string.IsNullOrWhiteSpace(brandName) || !string.IsNullOrWhiteSpace(BrandName))
            return this;
        BrandName = brandName;
        return this;
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}

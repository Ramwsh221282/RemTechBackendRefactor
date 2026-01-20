using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Models.Queries.GetModels;

public class GetModelsQuery : IQuery
{
    public Guid? BrandId { get; private init; }
    public string? BrandName { get; private init; }
    public Guid? CategoryId { get; private init; }
    public string? CategoryName { get; private init; }

    public GetModelsQuery ForBrandId(Guid? brandId) =>
        brandId == null || brandId == Guid.Empty ? this : Copy(this, brandId: brandId);

    public GetModelsQuery ForBrandName(string? brandName) =>
        string.IsNullOrWhiteSpace(brandName) ? this : Copy(this, brandName: brandName);

    public GetModelsQuery ForCategoryId(Guid? categoryId) =>
        categoryId == null || categoryId == Guid.Empty ? this : Copy(this, categoryId: categoryId);

    public GetModelsQuery ForCategoryName(string? categoryName) =>
        string.IsNullOrWhiteSpace(categoryName) ? this : Copy(this, categoryName: categoryName);

    private static GetModelsQuery Copy(
        GetModelsQuery origin,
        Guid? brandId = null,
        string? brandName = null,
        Guid? categoryId = null,
        string? categoryName = null
    ) =>
        new()
        {
            BrandId = brandId ?? origin.BrandId,
            BrandName = brandName ?? origin.BrandName,
            CategoryId = categoryId ?? origin.CategoryId,
            CategoryName = categoryName ?? origin.CategoryName,
        };

    public override string ToString() => JsonSerializer.Serialize(this);
}

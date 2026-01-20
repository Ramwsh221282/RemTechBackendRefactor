using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class GetBrandsQuery : IQuery
{
    public Guid? CategoryId { get; private init; }
    public string? CategoryName { get; private init; }
    public Guid? ModelId { get; private init; }
    public string? ModelName { get; private init; }
    public Guid? Id { get; private init; }
    public string? Name { get; private init; }

    public GetBrandsQuery ForId(Guid? id) =>
        id == null || id == Guid.Empty ? this : Copy(this, id: id);

    public GetBrandsQuery ForName(string? name) =>
        string.IsNullOrWhiteSpace(name) ? this : Copy(this, name: name);

    public GetBrandsQuery ForCategoryId(Guid? categoryId) =>
        categoryId == null || categoryId == Guid.Empty ? this : Copy(this, categoryId: categoryId);

    public GetBrandsQuery ForCategoryName(string? categoryName) =>
        string.IsNullOrWhiteSpace(categoryName) ? this : Copy(this, categoryName: categoryName);

    public GetBrandsQuery ForModelId(Guid? modelId) =>
        modelId == null || modelId == Guid.Empty ? this : Copy(this, modelId: modelId);

    public GetBrandsQuery ForModelName(string? modelName) =>
        string.IsNullOrWhiteSpace(modelName) ? this : Copy(this, modelName: modelName);

    private static GetBrandsQuery Copy(
        GetBrandsQuery origin,
        Guid? id = null,
        string? name = null,
        Guid? categoryId = null,
        string? categoryName = null,
        Guid? modelId = null,
        string? modelName = null
    ) =>
        new()
        {
            CategoryId = categoryId ?? origin.CategoryId,
            CategoryName = categoryName ?? origin.CategoryName,
            ModelId = modelId ?? origin.ModelId,
            ModelName = modelName ?? origin.ModelName,
            Id = id ?? origin.Id,
            Name = name ?? origin.Name,
        };
}

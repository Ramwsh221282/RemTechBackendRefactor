using System.Text.Json.Serialization;
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
    public IEnumerable<string>? Includes { get; private init; }
    public int? Page { get; private set; }
    public int? PageSize { get; private set; }
    public string? TextSearch { get; private set; }

    [JsonIgnore]
    private Dictionary<string, string> IncludedInformationKeys =>
        Includes is null ? [] : Includes.ToDictionary(i => i, i => i);

    public bool ContainsFieldInclude(string include) =>
        IncludedInformationKeys.ContainsKey(include);

    public GetBrandsQuery WithInclude(IEnumerable<string>? includes) =>
        includes is null || !includes.Any() ? this : Copy(this, includes: includes);

    public GetBrandsQuery WithPagination(int? page) =>
        page == null || page <= 0 ? this : Copy(this, page: page);

    public GetBrandsQuery WithPageSize(int? pageSize) =>
        pageSize == null || pageSize >= 30 ? this : Copy(this, pageSize: pageSize);

    public GetBrandsQuery WithTextSearch(string? textSearch) =>
        string.IsNullOrWhiteSpace(textSearch) ? this : Copy(this, textSearch: textSearch);

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
        string? modelName = null,
        IEnumerable<string>? includes = null,
        int? page = null,
        int? pageSize = null,
        string? textSearch = null
    ) =>
        new()
        {
            CategoryId = categoryId ?? origin.CategoryId,
            CategoryName = categoryName ?? origin.CategoryName,
            ModelId = modelId ?? origin.ModelId,
            ModelName = modelName ?? origin.ModelName,
            Id = id ?? origin.Id,
            Name = name ?? origin.Name,
            Includes = includes ?? origin.Includes,
            Page = page ?? origin.Page,
            PageSize = pageSize ?? origin.PageSize,
            TextSearch = textSearch ?? origin.TextSearch,
        };
}

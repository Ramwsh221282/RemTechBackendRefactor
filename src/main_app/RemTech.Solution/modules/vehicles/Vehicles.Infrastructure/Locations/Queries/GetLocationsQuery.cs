using System.Text.Json;
using System.Text.Json.Serialization;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Locations.Queries;

public sealed class GetLocationsQuery : IQuery
{
    // private constructor. Use static methods for creation.
    private GetLocationsQuery() { }

    public int? Amount { get; private init; } = 20;
    public string? TextSearch { get; private init; } = null;
    public Guid? Id { get; private init; } = null;
    public Guid? CategoryId { get; private init; } = null;
    public Guid? BrandId { get; private init; } = null;
    public Guid? ModelId { get; private init; } = null;
    public string? CategoryName { get; private init; } = null;
    public string? BrandName { get; private init; } = null;
    public string? ModelName { get; private init; } = null;
    public IEnumerable<string>? Includes { get; private init; } = null;

    [JsonIgnore]
    private Dictionary<string, string>? _includedInformationKeys_cached = null;

    [JsonIgnore]
    private Dictionary<string, string> IncludedInformationKeys =>
        _includedInformationKeys_cached ??= Includes is null
            ? []
            : Includes.ToHashSet().ToDictionary(i => i, i => i);

    public GetLocationsQuery WithId(Guid? id) => Copy(this, id: id);

    public GetLocationsQuery WithAmount(int? amount) => Copy(this, amount: amount);

    public GetLocationsQuery WithCategoryId(Guid? categoryId) => Copy(this, categoryId: categoryId);

    public GetLocationsQuery WithBrandId(Guid? brandId) => Copy(this, brandId: brandId);

    public GetLocationsQuery WithModelId(Guid? modelId) => Copy(this, modelId: modelId);

    public GetLocationsQuery WithCategoryName(string? categoryName) =>
        Copy(this, categoryName: categoryName);

    public GetLocationsQuery WithBrandName(string? brandName) => Copy(this, brandName: brandName);

    public GetLocationsQuery WithModelName(string? modelName) => Copy(this, modelName: modelName);

    public GetLocationsQuery WithTextSearch(string? textSearch) =>
        Copy(this, textSearch: textSearch);

    public GetLocationsQuery WithIncludes(IEnumerable<string>? includes) =>
        Copy(this, includes: includes);

    public bool ContainsInclude(string includeName) =>
        IncludedInformationKeys.ContainsKey(includeName);

    public bool ContainsCategoryFilter() =>
        CategoryId != null && CategoryId.Value != Guid.Empty
        || !string.IsNullOrWhiteSpace(CategoryName);

    public bool ContainsBrandFilter() =>
        BrandId != null && BrandId.Value != Guid.Empty || !string.IsNullOrWhiteSpace(BrandName);

    public bool ContainsModelFilter() =>
        ModelId != null && ModelId.Value != Guid.Empty || !string.IsNullOrWhiteSpace(ModelName);

    private static GetLocationsQuery Copy(
        GetLocationsQuery original,
        int? amount = null,
        string? textSearch = null,
        Guid? id = null,
        Guid? categoryId = null,
        Guid? brandId = null,
        Guid? modelId = null,
        string? categoryName = null,
        string? brandName = null,
        string? modelName = null,
        IEnumerable<string>? includes = null
    ) =>
        new()
        {
            Amount = amount ?? original.Amount,
            TextSearch = textSearch ?? original.TextSearch,
            Id = id ?? original.Id,
            CategoryId = categoryId ?? original.CategoryId,
            BrandId = brandId ?? original.BrandId,
            ModelId = modelId ?? original.ModelId,
            CategoryName = categoryName ?? original.CategoryName,
            BrandName = brandName ?? original.BrandName,
            ModelName = modelName ?? original.ModelName,
            Includes = includes is null ? original.Includes : [.. includes],
        };

    public static GetLocationsQuery Create() => new() { Amount = 20 };

    public override string ToString() => JsonSerializer.Serialize(this);
}

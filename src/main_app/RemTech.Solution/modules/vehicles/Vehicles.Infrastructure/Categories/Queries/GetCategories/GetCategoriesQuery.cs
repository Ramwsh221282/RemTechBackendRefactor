using System.Text.Json;
using System.Text.Json.Serialization;
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
	public IEnumerable<string>? IncludedInformation { get; private set; }

	public int? Page { get; private set; }
	public int? PageSize { get; private set; }
	public string? TextSearch { get; private set; }

	[JsonIgnore]
	private Dictionary<string, string>? _includedInformationKeys_cached = null;

	[JsonIgnore]
	private Dictionary<string, string> IncludedInformationKeys =>
		IncludedInformation is null
			? []
			: _includedInformationKeys_cached ??= IncludedInformation.ToHashSet().ToDictionary(i => i, i => i);

	public GetCategoriesQuery WithPage(int? page) => Copy(this, page: page);

	public GetCategoriesQuery WithPageSize(int? pageSize) => Copy(this, pageSize: pageSize);

	public GetCategoriesQuery WithTextSearch(string? textSearch) => Copy(this, textSearch: textSearch);

	public GetCategoriesQuery WithIncludedInformation(IEnumerable<string>? includedInformation) =>
		Copy(this, includedInformation: includedInformation);

	public GetCategoriesQuery ForId(Guid? id) => id == null || id == Guid.Empty ? this : Copy(this, id: id);

	public GetCategoriesQuery ForName(string? name) => string.IsNullOrWhiteSpace(name) ? this : Copy(this, name: name);

	public GetCategoriesQuery ForBrandId(Guid? brandId) =>
		brandId == null || brandId == Guid.Empty ? this : Copy(this, brandId: brandId);

	public GetCategoriesQuery ForBrandName(string? brandName) =>
		string.IsNullOrWhiteSpace(brandName) ? this : Copy(this, brandName: brandName);

	public GetCategoriesQuery ForModelId(Guid? modelId) =>
		modelId == null || modelId == Guid.Empty ? this : Copy(this, modelId: modelId);

	public GetCategoriesQuery ForModelName(string? modelName) =>
		string.IsNullOrWhiteSpace(modelName) ? this : Copy(this, modelName: modelName);

	public override string ToString() => JsonSerializer.Serialize(this);

	public bool ContainsIncludedInformationKey(string key) => IncludedInformationKeys.ContainsKey(key);

	private static GetCategoriesQuery Copy(
		GetCategoriesQuery origin,
		Guid? brandId = null,
		string? brandName = null,
		Guid? modelId = null,
		string? modelName = null,
		Guid? id = null,
		string? name = null,
		IEnumerable<string>? includedInformation = null,
		int? page = null,
		int? pageSize = null,
		string? textSearch = null
	) =>
		new()
		{
			BrandId = brandId ?? origin.BrandId,
			BrandName = brandName ?? origin.BrandName,
			ModelId = modelId ?? origin.ModelId,
			ModelName = modelName ?? origin.ModelName,
			Id = id ?? origin.Id,
			Name = name ?? origin.Name,
			IncludedInformation = includedInformation is null
				? origin.IncludedInformation
				: [.. includedInformation, .. origin.IncludedInformation ?? []],
			Page = page ?? origin.Page,
			PageSize = pageSize ?? origin.PageSize,
			TextSearch = textSearch ?? origin.TextSearch,
		};
}

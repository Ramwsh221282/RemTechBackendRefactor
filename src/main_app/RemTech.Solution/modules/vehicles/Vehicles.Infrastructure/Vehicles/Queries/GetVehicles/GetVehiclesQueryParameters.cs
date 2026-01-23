using System.Text.Json;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesQueryParameters
{
	public Guid? BrandId { get; private set; } = null;
	public Guid? CategoryId { get; private set; } = null;
	public Guid? RegionId { get; private set; } = null;
	public Guid? ModelId { get; private set; } = null;
	public bool? IsNds { get; private set; } = null;
	public long? MinimalPrice { get; private set; } = null;
	public long? MaximalPrice { get; private set; } = null;
	public IEnumerable<string>? SortFields { get; private set; } = null;
	public string Sort { get; private set; } = "NONE";
	public int Page { get; private set; } = 1;
	public int PageSize { get; private set; } = 50;
	public string? TextSearch { get; private set; }
	public Dictionary<Guid, string>? Characteristics { get; private set; } = null;

	public GetVehiclesQueryParameters ForBrand(Guid? brandId)
	{
		if (BrandId is not null)
			return this;
		if (brandId is null)
			return this;
		BrandId = brandId;
		return this;
	}

	public GetVehiclesQueryParameters ForCategory(Guid? categoryId)
	{
		if (CategoryId is not null)
			return this;
		if (categoryId is null)
			return this;
		CategoryId = categoryId;
		return this;
	}

	public GetVehiclesQueryParameters ForCharacteristics<T>(IEnumerable<T>? source, Func<T, (Guid, string)> converter)
	{
		if (Characteristics is not null)
			return this;
		if (source is null)
			return this;
		Dictionary<Guid, string> characteristics = [];
		foreach (T entry in source)
		{
			(Guid ctxId, string ctxName) = converter(entry);
			characteristics.TryAdd(ctxId, ctxName);
		}
		Characteristics = characteristics;
		return this;
	}

	public GetVehiclesQueryParameters ForTextSearch(string? textSearch)
	{
		if (!string.IsNullOrWhiteSpace(TextSearch))
			return this;
		if (string.IsNullOrEmpty(textSearch))
			return this;
		TextSearch = textSearch.Trim();
		return this;
	}

	public GetVehiclesQueryParameters ForRegion(Guid? regionId)
	{
		if (RegionId is not null)
			return this;
		if (regionId is null)
			return this;
		RegionId = regionId;
		return this;
	}

	public GetVehiclesQueryParameters ForModel(Guid? modelId)
	{
		if (ModelId is not null)
			return this;
		if (modelId is null)
			return this;
		ModelId = modelId;
		return this;
	}

	public GetVehiclesQueryParameters ForNds(bool? isNds)
	{
		if (IsNds is not null)
			return this;
		if (isNds is null)
			return this;
		IsNds = isNds;
		return this;
	}

	public GetVehiclesQueryParameters ForMinimalPrice(long? minimalPrice)
	{
		if (MinimalPrice is not null)
			return this;
		if (minimalPrice is null)
			return this;
		MinimalPrice = minimalPrice;
		return this;
	}

	public GetVehiclesQueryParameters ForMaximalPrice(long? maximalPrice)
	{
		if (MaximalPrice is not null)
			return this;
		if (maximalPrice is null)
			return this;
		MaximalPrice = maximalPrice;
		return this;
	}

	public GetVehiclesQueryParameters ForSort(string? sort)
	{
		if (sort is null)
			return this;
		Sort = sort;
		return this;
	}

	public GetVehiclesQueryParameters ForSortFields(IEnumerable<string>? sortFields)
	{
		if (sortFields is null)
			return this;
		SortFields = sortFields;
		return this;
	}

	public GetVehiclesQueryParameters ForPage(int page)
	{
		if (page <= 0)
			return this;
		Page = page;
		return this;
	}

	public GetVehiclesQueryParameters ForPageSize(int pageSize)
	{
		if (pageSize <= 50)
			return this;
		PageSize = pageSize;
		return this;
	}

	public override string ToString() => JsonSerializer.Serialize(this);
}

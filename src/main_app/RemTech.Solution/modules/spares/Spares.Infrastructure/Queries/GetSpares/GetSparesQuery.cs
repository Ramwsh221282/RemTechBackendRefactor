using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Infrastructure.Queries.GetSpares;

public sealed class GetSparesQuery : IQuery
{
	public Guid? RegionId { get; private set; } = null;
	public long? PriceMin { get; private set; } = null;
	public long? PriceMax { get; private set; } = null;
	public string? TextSearch { get; private set; } = null;
	public int Page { get; private set; } = 1;
	public int PageSize { get; private set; } = 50;
	public string OrderMode { get; private set; } = "NONE";

	public string? Oem { get; private set; } = null;

	public GetSparesQuery ForRegion(Guid? regionId)
	{
		if (RegionId.HasValue)
			return this;
		if (regionId is null)
			return this;
		RegionId = regionId;
		return this;
	}

	public GetSparesQuery ForOem(string? oem)
	{
		if (!string.IsNullOrWhiteSpace(Oem))
			return this;
		if (string.IsNullOrWhiteSpace(oem))
			return this;
		Oem = oem;
		return this;
	}

	public GetSparesQuery WithMinimalPrice(long? price)
	{
		if (PriceMin.HasValue)
			return this;
		if (price is null)
			return this;
		PriceMin = price;
		return this;
	}

	public GetSparesQuery WithMaximalPrice(long? price)
	{
		if (PriceMax.HasValue)
			return this;
		if (price is null)
			return this;
		PriceMax = price;
		return this;
	}

	public GetSparesQuery WithTextSearch(string? text)
	{
		if (!string.IsNullOrWhiteSpace(TextSearch))
			return this;
		if (string.IsNullOrWhiteSpace(text))
			return this;
		TextSearch = text;
		return this;
	}

	public GetSparesQuery WithOrderMode(string? mode)
	{
		if (string.IsNullOrWhiteSpace(mode))
			return this;
		OrderMode = mode;
		return this;
	}

	public GetSparesQuery WithPage(int? page)
	{
		if (page is null)
			return this;
		if (page.Value <= 0)
			return this;
		Page = page.Value;
		return this;
	}

	public GetSparesQuery WithPageSize(int? size)
	{
		if (size is null)
			return this;
		if (size.Value > 50)
			return this;
		PageSize = size.Value;
		return this;
	}
}

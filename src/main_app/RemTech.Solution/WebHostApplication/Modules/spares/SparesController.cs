using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Spares.Infrastructure.Queries.GetSpares;
using Spares.Infrastructure.Queries.GetSparesLocations;
using Spares.Infrastructure.Queries.GetSpareTypes;

namespace WebHostApplication.Modules.spares;

[ApiController]
[Route("api/spares")]
public sealed class SparesController
{
	[HttpGet]
	public async Task<Envelope> GetSpares(
		[FromQuery(Name = "region-id")] Guid? regionId,
		[FromQuery(Name = "price-min")] long? priceMin,
		[FromQuery(Name = "price-max")] long? priceMax,
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "page")] int? page,
		[FromQuery(Name = "page-size")] int? pageSize,
		[FromQuery(Name = "sort-mode")] string? sortMode,
		[FromQuery(Name = "oem")] string? oem,
		[FromQuery(Name = "type")] string? type,
		[FromServices] IQueryHandler<GetSparesQuery, GetSparesQueryResponse> handler,
		CancellationToken ct
	)
	{
		GetSparesQuery query = new GetSparesQuery()
			.ForRegion(regionId)
			.ForOem(oem)
			.WithMinimalPrice(priceMin)
			.WithMaximalPrice(priceMax)
			.WithTextSearch(textSearch)
			.WithPage(page)
			.WithPageSize(pageSize)
			.WithOrderMode(sortMode)
			.ForType(type);
		GetSparesQueryResponse result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}

	[HttpGet("locations")]
	public async Task<Envelope> GetSparesLocations(
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromServices] IQueryHandler<GetSparesLocationsQuery, IEnumerable<SpareLocationResponse>> handler,
		CancellationToken ct
	)
	{
		GetSparesLocationsQuery query = GetSparesLocationsQuery.Create().WithTextSearch(textSearch);
		IEnumerable<SpareLocationResponse> result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}

	[HttpGet("types")]
	public async Task<Envelope> GetSpareTypes(
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "amount")] int? amount,
		[FromServices] IQueryHandler<GetSpareTypesQuery, IEnumerable<SpareTypeResponse>> handler,
		CancellationToken ct
	)
	{
		GetSpareTypesQuery query = GetSpareTypesQuery.Create().WithTextSearch(textSearch).WithAmount(amount);
		IEnumerable<SpareTypeResponse> result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}
}

using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Spares.Infrastructure.Queries.GetSpares;

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
        [FromServices] IQueryHandler<GetSparesQuery, GetSparesQueryResponse> handler,
        CancellationToken ct)
    {
        GetSparesQuery query = new GetSparesQuery()
            .ForRegion(regionId)
            .ForOem(oem)
            .WithMinimalPrice(priceMin)
            .WithMaximalPrice(priceMax)
            .WithTextSearch(textSearch)
            .WithPage(page)
            .WithPageSize(pageSize)
            .WithOrderMode(sortMode);
        GetSparesQueryResponse result = await handler.Handle(query, ct);
        return EnvelopedResultsExtensions.AsEnvelope(result);
    }
}
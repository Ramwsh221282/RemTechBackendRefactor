using ContainedItems.Infrastructure.Queries.GetMainPageItemStats;
using ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;
using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;

namespace WebHostApplication.Modules.ContainedItems;

[ApiController]
[Route("api/contained-items")]
public sealed class ContainedItemsController
{
    [HttpGet("main-page/statistics")]
    public async Task<Envelope> GetStatistics(
        [FromServices] IQueryHandler<GetMainPageItemStatsQuery, MainPageItemStatsResponse> handler,
        CancellationToken ct = default
    )
    {
        GetMainPageItemStatsQuery query = new();
        MainPageItemStatsResponse response = await handler.Handle(query, ct);
        return EnvelopedResultsExtensions.AsEnvelope(response);
    }

    [HttpGet("main-page/last-added")]
    public async Task<Envelope> GetLastAddedItems(
        [FromServices]
            IQueryHandler<GetMainPageLastAddedItemsQuery, MainPageLastAddedItemsResponse> handler,
        CancellationToken ct = default
    )
    {
        GetMainPageLastAddedItemsQuery query = new();
        MainPageLastAddedItemsResponse response = await handler.Handle(query, ct);
        return EnvelopedResultsExtensions.AsEnvelope(response);
    }
}

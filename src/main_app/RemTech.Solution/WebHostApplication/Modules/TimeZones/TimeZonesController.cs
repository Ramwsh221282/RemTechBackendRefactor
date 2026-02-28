using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Timezones.Core.Models;
using TimeZones.Infrastructure;

namespace WebHostApplication.Modules.TimeZones;


[ApiController]
[Route("api/timezones")]
public sealed class TimeZonesController : ControllerBase
{
    [HttpGet]
    public async Task<Envelope> GetTimeZones(
        [FromServices] IQueryHandler<GetRegionLocalDateTimesQuery, IReadOnlyList<RegionLocalDateTime>> handler,
        CancellationToken ct = default
    )
    {
        GetRegionLocalDateTimesQuery query = new();
        IReadOnlyList<RegionLocalDateTime> records = await handler.Handle(query, ct);        
        return EnvelopedResultsExtensions.AsEnvelope(records);
    }
}

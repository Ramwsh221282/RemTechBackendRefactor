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
        [FromServices] IQueryHandler<GetTimeZonesQuery, IReadOnlyList<TimeZoneRecord>> handler,
        CancellationToken ct = default
    )
    {
        GetTimeZonesQuery query = new();
        IReadOnlyList<TimeZoneRecord> records = await handler.Handle(query, ct);
        IReadOnlyList<TimeZoneModuleResponses.TimeZoneRecordResponse> response = TimeZoneModuleResponses.TimeZoneRecordResponse.From(records);
        return EnvelopedResultsExtensions.AsEnvelope(response);
    }
}

using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Timezones.Core.Features.ChangeApplicationTimeZone;
using Timezones.Core.Features.SelectApplicationTimeZone;
using Timezones.Core.Models;
using TimeZones.Infrastructure.Queries.GetCurrentRegionLocalDateTime;
using TimeZones.Infrastructure.Queries.GetRegionLocalDateTimes;

namespace WebHostApplication.Modules.TimeZones;

// TODO: добавит атрибуты прав на использование этого контроллера (пермиссии)
// TODO: добавить сидирование пермиссии для управления часовым поясом приложения.

[ApiController]
[Route("api/timezones")]
public sealed class TimeZonesController : ControllerBase
{
	[HttpGet("list")]
	public async Task<Envelope> GetTimeZones(
		[FromServices] IQueryHandler<GetRegionLocalDateTimesQuery, IReadOnlyList<RegionLocalDateTime>> handler,
		CancellationToken ct = default
	)
	{
		GetRegionLocalDateTimesQuery query = new();
		IReadOnlyList<RegionLocalDateTime> records = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(records);
	}

	[HttpGet]
	public async Task<Envelope> GetCurrentTimeZone(
		[FromServices] IQueryHandler<GetCurrentRegionLocalDateTimeQuery, CurrentRegionLocalDateTime?> handler,
		CancellationToken ct = default
	)
	{
		GetCurrentRegionLocalDateTimeQuery query = new();
		CurrentRegionLocalDateTime? record = await handler.Handle(query, ct);
		if (record is null)
		{
			Result notFound = Result.Failure(Error.NotFound("Текущая временная зона не установлена"));
			return EnvelopedResultsExtensions.AsEnvelope(notFound);
		}

		return EnvelopedResultsExtensions.AsEnvelope(record);
	}

	[HttpPut("{zoneName}")]
	public async Task<Envelope> ChangeCurrentTimeZone(
		[FromRoute(Name = "zoneName")] string currentName,
		[FromQuery(Name = "name")] string newName,
		[FromServices] ICommandHandler<ChangeApplicationTimeZoneCommand, Unit> handler,
		CancellationToken ct = default
	)
	{
		ChangeApplicationTimeZoneCommand command = new(CurrentZoneName: currentName, NewZoneName: newName);
        Result<Unit> result = await handler.Execute(command, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}

	[HttpPost]
	public async Task<Envelope> SetCurrentTimeZone(
		[FromQuery(Name = "name")] string zoneName,
		[FromServices] ICommandHandler<SelectApplicationTimeZoneCommand, RegionLocalDateTime> handler,
		CancellationToken ct = default
	)
	{
		SelectApplicationTimeZoneCommand command = new(zoneName);
        Result<RegionLocalDateTime> result = await handler.Execute(command, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}
}

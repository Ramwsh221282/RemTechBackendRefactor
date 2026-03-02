using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace Timezones.Core.Features.ChangeApplicationTimeZone;

public sealed record ChangeApplicationTimeZoneCommand(string CurrentZoneName, string NewZoneName) : ICommand;

[TransactionalHandler]
public sealed class ChangeApplicationTimeZoneHandler : ICommandHandler<ChangeApplicationTimeZoneCommand, Unit>
{
    public ChangeApplicationTimeZoneHandler(
        ITimeZonesProvider timeZones,
        IRegionDateTimesRepository regionDateTimes)
    {
        _timeZonesProvider = timeZones;
        _regionDateTimes = regionDateTimes;
    }

    private readonly ITimeZonesProvider _timeZonesProvider;
    private readonly IRegionDateTimesRepository _regionDateTimes;

    public async Task<Result<Unit>> Execute(
        ChangeApplicationTimeZoneCommand command, 
        CancellationToken ct = default)
    {
        TimeZoneRecord? @new = await _timeZonesProvider.FetchTimeZone(command.NewZoneName, ct);
        if (@new is null)
        {
            return Error.NotFound($"Временная зона: {command.NewZoneName} не найдена");
        }

        TimeZoneRecord? current = await _timeZonesProvider.FetchTimeZone(command.CurrentZoneName, ct);
        if (current is null)
        {
            return Error.NotFound($"Временная зона: {command.CurrentZoneName} не найдена");
        }

        RegionLocalDateTime dateTime = @new.ToRegionLocalDateTime();
        await _regionDateTimes.Save(dateTime, ct);
        return Unit.Value;
    }
}

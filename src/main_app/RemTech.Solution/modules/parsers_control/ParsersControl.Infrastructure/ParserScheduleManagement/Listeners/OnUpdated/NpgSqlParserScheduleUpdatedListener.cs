using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnUpdated;

public sealed class NpgSqlParserScheduleUpdatedListener(IParserScheduleStorage storage) : IParserScheduleUpdatedEventListener
{
    public async Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default)
    {
        await storage.Update(new ParserSchedule(data), ct);
        return Unit.Value;
    }
}
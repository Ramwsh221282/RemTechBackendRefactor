using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using ParsersControl.Infrastructure.ParserScheduleManagement.NpgSql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnCreated;

public sealed class NpgSqlParserScheduleCreatedListener(IParserScheduleStorage storage) : IParserScheduleCreatedEventListener
{
    public async Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default)
    {
        Result<Unit> validation = await data.ValidateUniquesness(storage, ct);
        if (validation.IsFailure) return validation.Error;
        await storage.Persist(new ParserSchedule(data), ct);
        return Unit.Value;
    }
}
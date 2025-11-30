using ParsersControl.Core.ParserScheduleManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserScheduleManagement.Defaults;

internal sealed class NoneParserScheduleCreatedEventListener : IParserScheduleCreatedEventListener
{
    public Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
using ParsersControl.Core.ParserWorkTurning.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning.Defaults;

public sealed class NoneOnParsedStartWaitingEventListener : IOnParserStartWaitingEventListener
{
    public Task<Result<Unit>> React(ParserWorkTurnerState workTurnerState, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
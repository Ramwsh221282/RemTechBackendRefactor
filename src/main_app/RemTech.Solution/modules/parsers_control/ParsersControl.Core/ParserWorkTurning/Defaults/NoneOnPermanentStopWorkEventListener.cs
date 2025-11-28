using ParsersControl.Core.ParserWorkTurning.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning.Defaults;

public sealed class NoneOnPermanentStopWorkEventListener : IOnPermanentStopWorkEventListener
{
    public Task<Result<Unit>> React(ParserWorkTurnerState state, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
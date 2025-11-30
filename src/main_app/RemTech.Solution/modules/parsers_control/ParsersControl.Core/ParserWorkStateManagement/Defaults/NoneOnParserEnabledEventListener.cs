using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkStateManagement.Defaults;

public sealed class NoneOnParserEnabledEventListener : IOnParserEnabledEventListener
{
    public Task<Result<Unit>> React(ParserWorkTurnerState workTurnerState, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
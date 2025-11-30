using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserStateManagement.Defaults;

public sealed class NoneOnStatefulParserStateChangedEventListener : IOnStatefulParserStateChangedEventListener
{
    public Task<Result<Unit>> React(
        RegisteredParser parser,
        ParserWorkTurner turner,
        CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
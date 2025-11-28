using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserRegistrationManagement.Defaults;

public sealed class NoneOnParserRegisteredEventListener : IOnParserRegisteredEventListener
{
    public Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
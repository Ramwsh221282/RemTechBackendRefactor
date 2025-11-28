using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinksManagement.Defaults;

public sealed class NoneParserLinkUrlChangedListener : IParserLinkUrlChangedListener
{
    public Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinksManagement.Defaults;

public sealed class NoneParserLinkAttachedListener : IParserLinkParserAttached
{
    public Task<Result<Unit>> React(Guid parserId, ParserLinkData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
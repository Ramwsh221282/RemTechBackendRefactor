using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkParserAttachedListener;

public sealed class NpgSqlParserLinkAttachedListener(IParserLinksStorage storage) : IParserLinkParserAttached
{
    public async Task<Result<Unit>> React(Guid parserId, ParserLinkData data, CancellationToken ct = default)
    {
        Result<Unit> validation = await data.ParserAlreadyHasLink(parserId, storage, ct);
        if (validation.IsFailure) return validation.Error;
        await storage.Persist(new ParserLink(data), ct);
        return Unit.Value;
    }
}
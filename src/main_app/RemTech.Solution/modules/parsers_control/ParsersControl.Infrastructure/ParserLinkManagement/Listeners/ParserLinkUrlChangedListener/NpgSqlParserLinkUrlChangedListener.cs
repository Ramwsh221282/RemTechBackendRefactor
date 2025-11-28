using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUrlChangedListener;

public sealed class NpgSqlParserLinkUrlChangedListener(IParserLinksStorage storage) : IParserLinkUrlChangedListener
{
    public async Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        Result<Unit> validation = await data.ParserAlreadyHasLink(data.ParserId, storage, ct);
        if (validation.IsFailure) return validation.Error;
        await storage.Update(new ParserLink(data), ct);
        return Unit.Value;
    }
}
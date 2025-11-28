using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUnignoredListener;

public sealed class NpgSqlParserLinkUnignoredListener(IParserLinksStorage storage) : IParserLinkUnignoredListener
{
    public async Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        await storage.Update(new ParserLink(data), ct);
        return Unit.Value;
    }
}
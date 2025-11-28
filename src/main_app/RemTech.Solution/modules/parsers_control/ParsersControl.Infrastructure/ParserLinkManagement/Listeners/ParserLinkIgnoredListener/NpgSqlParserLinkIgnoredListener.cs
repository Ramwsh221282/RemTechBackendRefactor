using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkIgnoredListener;

public sealed class NpgSqlParserLinkIgnoredListener(IParserLinksStorage storage) : IParserLinkIgnoredListener
{
    public async Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        ParserLink link = new(data);
        await storage.Update(link, ct);
        return Unit.Value;
    }
}
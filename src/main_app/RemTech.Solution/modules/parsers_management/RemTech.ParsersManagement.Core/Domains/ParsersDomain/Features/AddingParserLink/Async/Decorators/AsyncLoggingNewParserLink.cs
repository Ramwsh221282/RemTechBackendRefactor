using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;

public sealed class AsyncLoggingNewParserLink(ILogger logger, IAsyncNewParserLink inner)
    : IAsyncNewParserLink
{
    public async Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default)
    {
        logger.Information("Асинхронное добавление новой ссылки парсеру начато.");
        Status<IParserLink> link = await inner.AsyncNew(add, ct);
        logger.Information("Асинхронное добавление новой ссылки парсеру закончено.");
        return link;
    }
}

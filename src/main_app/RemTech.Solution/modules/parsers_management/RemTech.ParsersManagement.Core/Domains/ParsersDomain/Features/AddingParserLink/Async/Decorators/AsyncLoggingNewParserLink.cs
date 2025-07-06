using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;

public sealed class AsyncLoggingNewParserLink(ICustomLogger logger, IAsyncNewParserLink inner)
    : IAsyncNewParserLink
{
    public async Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default
    )
    {
        logger.Info("Асинхронное добавление ссылки парсеру.");
        Status<IParserLink> status = await inner.AsyncNew(add, ct);
        logger.Info("Асинхронное добавление ссылки парсеру завершено.");
        return status;
    }
}

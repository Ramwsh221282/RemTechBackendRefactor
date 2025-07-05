using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class AsyncLoggingEnabledParser(ICustomLogger logger, IAsyncEnabledParser inner)
    : IAsyncEnabledParser
{
    public async Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    )
    {
        logger.Info("Асинхронное включение парсера.");
        Status<IParser> parser = await inner.EnableAsync(enable, ct);
        if (parser.IsFailure)
        {
            logger.Error("Ошибка: {0}.", parser.Error);
            return parser;
        }
        logger.Info("Асинхронное включение парсера завершено.");
        return parser;
    }
}

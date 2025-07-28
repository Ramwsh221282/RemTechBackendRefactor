using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Decorators;

public sealed class LoggingStartedParser(ILogger logger, IStartedParser inner)
    : IStartedParser
{
    public Status<IParser> Started(StartParser start)
    {
        Status<IParser> started = inner.Started(start);
        if (started.IsSuccess)
        {
            logger.Information("Парсер запущен.");
            return started;
        }
        logger.Error("Ошибка: {0}.", started.Error.ErrorText);
        return started;
    }
}

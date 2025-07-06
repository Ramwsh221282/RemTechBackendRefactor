using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Decorators;

public sealed class LoggingStartedParser(ICustomLogger logger, IStartedParser inner)
    : IStartedParser
{
    public Status<IParser> Started(StartParser start)
    {
        IParser parser = start.TakeStarter();
        logger.Info(
            "Запуск парсера ID: {0}, название: {1}, тип: {2}, домен: {3}.",
            (Guid)parser.Identification().ReadId(),
            (string)parser.Identification().ReadName().NameString(),
            (string)parser.Identification().ReadType().Read(),
            (string)parser.Identification().Domain().Read().NameString()
        );
        Status<IParser> started = inner.Started(start);
        if (started.IsSuccess)
        {
            logger.Info("Парсер запущен.");
            return started;
        }
        logger.Error("Ошибка: {0}.", started.Error.ErrorText);
        return started;
    }
}

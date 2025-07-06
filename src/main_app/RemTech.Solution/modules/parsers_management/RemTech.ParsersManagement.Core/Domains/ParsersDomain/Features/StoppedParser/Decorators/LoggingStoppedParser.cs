using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Decorators;

public sealed class LoggingStoppedParser(ICustomLogger logger, IStoppedParser inner)
    : IStoppedParser
{
    public Status<IParser> Stopped(StopParser stop)
    {
        IParser parser = stop.WhomStop();
        logger.Info(
            "Остановка парсера с ID: {0}, название: {1}, тип: {2}, домен: {3}.",
            (Guid)parser.Identification().ReadId(),
            (string)parser.Identification().ReadName().NameString(),
            (string)parser.Identification().ReadType().Read(),
            (string)parser.Identification().Domain().Read().NameString()
        );
        Status<IParser> stopped = inner.Stopped(stop);
        if (stopped.IsSuccess)
        {
            logger.Info("Парсер остановлен.");
            return stopped;
        }
        logger.Error("Ошибка: {0}.", stopped.Error.ErrorText);
        return stopped;
    }
}

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
            parser.Identification().ReadId().GuidValue(),
            parser.Identification().ReadName().NameString().StringValue(),
            parser.Identification().ReadType().Read().StringValue(),
            parser.Identification().Domain().Read().NameString().StringValue()
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

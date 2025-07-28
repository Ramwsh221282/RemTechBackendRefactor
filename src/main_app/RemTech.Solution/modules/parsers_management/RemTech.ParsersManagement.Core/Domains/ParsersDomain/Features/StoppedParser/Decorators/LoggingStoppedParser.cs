using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Decorators;

public sealed class LoggingStoppedParser(ILogger logger, IStoppedParser inner)
    : IStoppedParser
{
    public Status<IParser> Stopped(StopParser stop)
    {
        IParser parser = stop.WhomStop();
        Status<IParser> stopped = inner.Stopped(stop);
        if (stopped.IsSuccess)
        {
            logger.Information("Парсер остановлен.");
            return stopped;
        }
        logger.Error("Ошибка: {0}.", stopped.Error.ErrorText);
        return stopped;
    }
}

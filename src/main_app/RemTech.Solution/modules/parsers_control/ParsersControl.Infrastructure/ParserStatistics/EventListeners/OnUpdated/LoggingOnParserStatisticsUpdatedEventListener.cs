using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnUpdated;

public sealed class LoggingOnParserStatisticsUpdatedEventListener(
    Serilog.ILogger logger
) : IParserStatisticsUpdatedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<LoggingOnParserStatisticsUpdatedEventListener>();
    
    public Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default)
    {
        ParserStatistic statistic = new(data);
        ParserStatisticsDataLog log = new(_logger, statistic);
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
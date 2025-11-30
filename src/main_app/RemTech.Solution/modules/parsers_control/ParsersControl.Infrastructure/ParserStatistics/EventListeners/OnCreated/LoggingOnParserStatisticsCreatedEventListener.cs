using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnCreated;

public sealed class LoggingOnParserStatisticsCreatedEventListener(
    Serilog.ILogger logger
)
    : IParserStatisticsCreatedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IParserStatisticsCreatedEventListener>();
    
    public Task<Result<Unit>> React(ParserStatisticData data, CancellationToken ct = default)
    {
        _logger.Information("Parser statistics saved");
        ParserStatisticsDataLog log = new(_logger, new ParserStatistic(data));
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
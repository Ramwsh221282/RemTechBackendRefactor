using ParsersControl.Core.ParserStatisticsManagement;

namespace ParsersControl.Infrastructure.ParserStatistics.EventListeners;

public sealed class ParserStatisticsDataLog
{
    private readonly Serilog.ILogger _logger;
    private Guid _id = Guid.Empty;
    private int _processed;
    private long _totalElapsedSeconds;
    private void AddId(Guid id) => _id = id;
    private void AddProcessed(int processed) => _processed = processed;
    private void AddTotalElapsedSeconds(long  totalElapsedSeconds) => _totalElapsedSeconds = totalElapsedSeconds;

    public void Log()
    {
        object[] properties = [_id, _processed, _totalElapsedSeconds];
        _logger.Information("""
                            Parser statistics info:
                            Parser Id: {Id}
                            Processed: {Processed}
                            Total Seconds: {TotalSeconds}
                            """, properties);
    }
    public ParserStatisticsDataLog(Serilog.ILogger logger, ParserStatistic statistic)
    {
        _logger = logger; 
        statistic.Write(
            writeId: AddId, 
            writeProcessed: AddProcessed, 
            writeElapsedSeconds: AddTotalElapsedSeconds
        );
    }
}
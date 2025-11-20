using RemTech.Outbox.Shared;

namespace Identity.Outbox.Decorators;

public sealed class LoggingTransactionalOutboxProcessorWork : IIdentityOutboxProcessorWork
{
    private readonly IIdentityOutboxProcessorWork _origin;
    private readonly Serilog.ILogger _logger;
    private const string Context = nameof(IIdentityOutboxProcessorWork);
    
    public async Task<ProcessedOutboxMessages> ProcessMessages()
    {
        _logger.Information("Beginning outbox processor work.");
        try
        {
            ProcessedOutboxMessages processed = await _origin.ProcessMessages();
            _logger.Information("{Context} processed: {Count} messages.", Context, processed.ProcessedCount);
        }
        catch(Exception ex)
        {
            _logger.Error(ex, "{Context} failed", Context);
        }
        _logger.Information("Outbox processor work completed.");
        return new ProcessedOutboxMessages();
    }

    public LoggingTransactionalOutboxProcessorWork(Serilog.ILogger logger, IIdentityOutboxProcessorWork origin)
    {
        _origin = origin;
        _logger = logger;
    }
}
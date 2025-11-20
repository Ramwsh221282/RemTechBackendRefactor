using Quartz;

namespace Identity.Outbox;

[DisallowConcurrentExecution]
public sealed class IdentityOutboxProcessor : IJob
{
    private readonly IIdentityOutboxProcessorWork _work;
    private readonly Serilog.ILogger _logger;
    private const string Context = nameof(IdentityOutboxProcessor);

    public IdentityOutboxProcessor(IIdentityOutboxProcessorWork work, Serilog.ILogger logger)
    {
        _work = work;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.Information("{Context} processing job", Context);
        try
        {
            await _work.ProcessMessages();
            _logger.Information("{Context} job processed", Context);
        }
        catch(Exception ex)
        {
            _logger.Information("{Context} job failed.", Context);
            _logger.Information("{Context} job error {Ex}.", Context, ex);
        }
    }
}
using Quartz;
using RemTech.Outbox.Shared;

namespace Identity.Outbox;

[DisallowConcurrentExecution]
[CronSchedule("*/5 * * * * ?")]
public sealed class IdentityOutboxProcessor(IIdentityOutboxProcessorWork work, Serilog.ILogger logger)
    : ICronScheduleJob
{
    private const string Context = nameof(IdentityOutboxProcessor);

    public async Task Execute(IJobExecutionContext context)
    {
        logger.Information("{Context} processing job", Context);
        try
        {
            await work.ProcessMessages();
            logger.Information("{Context} job processed", Context);
        }
        catch(Exception ex)
        {
            logger.Information("{Context} job failed.", Context);
            logger.Information("{Context} job error {Ex}.", Context, ex);
        }
    }
}
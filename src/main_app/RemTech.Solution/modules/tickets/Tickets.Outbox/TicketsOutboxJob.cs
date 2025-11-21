using Quartz;
using RemTech.Outbox.Shared;

namespace Tickets.Outbox;

[DisallowConcurrentExecution]
[CronSchedule("*/5 * * * * ?")]
public sealed class TicketsOutboxJob(ITicketsOutboxJobMethod method) : ICronScheduleJob
{
    public async Task Execute(IJobExecutionContext context) =>
        await method.ProcessMessages(context.CancellationToken);
}
using Quartz;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace Mailing.Infrastructure.InboxMessageProcessing;

[DisallowConcurrentExecution]
[CronSchedule("*/10 * * * * ?")]
public sealed class InboxMessagesProcessor(
    InboxMessagesProcessorProtocol procedureProtocol, 
    InboxMessagesProcessorProcedureDependencies dependencies)
    : ICronScheduleJob
{
    private const string Context = nameof(InboxMessagesProcessor);
    
    public async Task Execute(IJobExecutionContext context)
    {
        await using (dependencies)
        {
            dependencies.Execute(d => d.Logger.Information("{Context} processing messages started", Context));
            
            try
            {
                await dependencies.Execute(async (d, token) =>
                        await d.Session.GetTransaction(token), context.CancellationToken);
                
                await procedureProtocol.ProcessAsync(
                    mailerSource: ct => dependencies.GetMailer.AvailableBySendLimit(true, ct),
                    dependencies: dependencies, 
                    ct: context.CancellationToken);
                
                await dependencies.Execute(async (d, token) => await d.Session.UnsafeCommit(token),
                    context.CancellationToken);
            }
            catch (Exception ex)
            {
                dependencies.Execute(d => d.Logger.Error("{Context} failed. Error: {Ex}", Context, ex.Message));
            }
            
        }
    }
}
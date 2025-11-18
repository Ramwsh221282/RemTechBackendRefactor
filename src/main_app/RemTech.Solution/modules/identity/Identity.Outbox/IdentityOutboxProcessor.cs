using Quartz;

namespace Identity.Outbox;

public sealed class IdentityOutboxProcessor : IJob
{
    private readonly IdentityOutboxProcessorWork _work;

    public IdentityOutboxProcessor(IdentityOutboxProcessorWork work) => _work = work;
    
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Executing IdentityOutboxProcessor");
        await using (IdentityOutboxProcessorWork work = _work)
        {
            await work.ProcessMessages();
        }
    }
}
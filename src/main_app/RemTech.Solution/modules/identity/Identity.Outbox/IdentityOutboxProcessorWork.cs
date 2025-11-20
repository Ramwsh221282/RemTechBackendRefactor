using RemTech.Outbox.Shared;

namespace Identity.Outbox;

public sealed class IdentityOutboxProcessorWork : IIdentityOutboxProcessorWork
{
    private readonly OutboxService _service;
    private readonly IHowToProcessIdentityOutboxMessage _howTo;
    private readonly CancellationToken _ct;

    public async Task<ProcessedOutboxMessages> ProcessMessages()
    {
        IEnumerable<OutboxMessage> messages = await _service.GetPendingMessages(50, _ct);
        ProcessedOutboxMessages result = await PublishMessages(messages);
        await result.RemoveProcessedMessages(_service, _ct);
        return result;
    }

    private async Task<ProcessedOutboxMessages> PublishMessages(IEnumerable<OutboxMessage> messages)
    {
        ProcessedOutboxMessages result = new();
        foreach (OutboxMessage message in messages)
            await _howTo.ProcessMessage(result, message, _ct);
        return result;
    }
    
    public IdentityOutboxProcessorWork(
        OutboxService service, 
        IHowToProcessIdentityOutboxMessage howTo,
        CancellationToken ct)
    {
        _service = service;
        _howTo = howTo;
        _ct = ct;
    }
}
namespace RemTech.Outbox.Shared;

public sealed class ProcessedOutboxMessages
{
    private readonly List<OutboxMessage> processedMessages = [];
    
    public int ProcessedCount => processedMessages.Count;
    
    public void Add(OutboxMessage message)
    {
        processedMessages.Add(message);
    }

    public async Task RemoveProcessedMessages(OutboxService storage, CancellationToken ct)
    {
        if (processedMessages.Count > 0)
            await storage.RemoveMany(processedMessages, ct);
    }
}
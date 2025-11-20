namespace Identity.Outbox;

public sealed class IdentityMessageProcessingResult
{
    private readonly List<IdentityOutboxMessage> processedMessages = [];
    
    public int ProcessedCount => processedMessages.Count;
    
    public void AddProcessedMessage(IdentityOutboxMessage message)
    {
        processedMessages.Add(message);
    }

    public async Task RemoveProcessedMessages(IdentityOutboxStorage storage, CancellationToken ct)
    {
        if (processedMessages.Count > 0)
            await storage.RemoveMany(processedMessages, ct);
    }
}
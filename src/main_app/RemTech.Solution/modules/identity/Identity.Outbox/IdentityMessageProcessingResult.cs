namespace Identity.Outbox;

public sealed class IdentityMessageProcessingResult
{
    private readonly List<IdentityOutboxMessage> processedMessages = [];
    private readonly List<IdentityOutboxMessage> unprocessedMessages = [];

    public void ResolveProcessing(bool isProcessed, IdentityOutboxMessage message)
    {
        if (isProcessed)
            processedMessages.Add(message with { ProcessedAt = DateTime.UtcNow });
        else
            unprocessedMessages.Add(message with { ProcessedAt = DateTime.UtcNow });
    }

    public async Task RefreshMessagesInDatabase(IdentityOutboxStorage storage, CancellationToken ct)
    {
        if (processedMessages.Count > 0)
            await storage.RemoveMany(processedMessages, ct);
        if (unprocessedMessages.Count > 0)
            await storage.UpdateMany(unprocessedMessages, ct);
    }
}
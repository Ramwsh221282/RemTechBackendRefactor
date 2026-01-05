namespace Identity.Domain.Contracts.Outbox;

public sealed class OutboxMessage
(
    Guid id,
    string type,
    int retryCount,
    DateTime created,
    DateTime? sent,
    object payload
)
{
    private OutboxMessage(OutboxMessage message) 
        : this(message.Id, message.Type, message.RetryCount, message.Created, message.Sent, message.Payload) { }
    private OutboxMessage(string type, int retryCount, object payload) 
        : this(Guid.NewGuid(), type, retryCount, DateTime.UtcNow, null, payload) { }
    
    public Guid Id { get; } = id;
    public string Type { get; } = type;
    public int RetryCount { get; private set; } = retryCount;
    public DateTime Created { get; } = created;
    public DateTime? Sent { get; private set; } = sent;
    public object Payload { get; } = payload;
    
    public static OutboxMessage CreateNew<T>(string type, int retryCount, T payload) where T : IOutboxMessagePayload 
        => new(type, retryCount, payload);
    
    public static OutboxMessage CreateNew<T>(string type, T payload) where T : IOutboxMessagePayload 
        => new(type, 0, payload);

    public void MarkSent() => Sent = DateTime.UtcNow;
    public void IncreaseRetryCount() => RetryCount++;
    public OutboxMessage Clone() => new(this);
}
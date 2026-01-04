namespace Identity.Domain.Contracts;

public sealed class OutboxMessage<T>
(
    Guid id,
    string type,
    int retryCount,
    DateTime created,
    DateTime? sent,
    T payload
) where T : IOutboxMessagePayload
{
    private OutboxMessage(OutboxMessage<T> message) 
        : this(message.Id, message.Type, message.RetryCount, message.Created, message.Sent, message.Payload) { }
    private OutboxMessage(string type, int retryCount, T payload) 
        : this(Guid.NewGuid(), type, retryCount, DateTime.UtcNow, null, payload) { }
    
    public Guid Id { get; } = id;
    public string Type { get; } = type;
    public int RetryCount { get; private set; } = retryCount;
    public DateTime Created { get; } = created;
    public DateTime? Sent { get; private set; } = sent;
    public T Payload { get; } = payload;
    
    public static OutboxMessage<T> CreateNew(string type, int retryCount, T payload) 
        => new(type, retryCount, payload);

    public void MarkSent() => Sent = DateTime.UtcNow;
    public void IncreaseRetryCount() => RetryCount++;
    public OutboxMessage<T> Clone()
        => new(this);
}
namespace Identity.Domain.Contracts.Outbox;

public sealed class OutboxMessageSpecification
{
    public string? Type { get; private set; } = null;
    public DateTime? CreatedDateTime { get; private set; } = null;
    public DateTime? SentDateTime { get; private set; } = null;
    public bool? SentOnly { get; private set; } = null;
    public bool? NotSentOnly { get; private set; } = null;
    public bool? WithLock { get; private set; } = null;
    public int? Limit { get; private set; } = null;
    public int? RetryCountLessThan { get; private set; } = null;

    public OutboxMessageSpecification OfLimit(int limit)
    {
        if (Limit.HasValue) return this;
        Limit = limit;
        return this;
    }
    
    public OutboxMessageSpecification OfRetryCountLessThan(int retryCount)
    {
        if (RetryCountLessThan.HasValue) return this;
        RetryCountLessThan = retryCount;
        return this;
 
    }
    
    public OutboxMessageSpecification OfType(string type)
    {
        if (!string.IsNullOrWhiteSpace(Type)) return this;
        Type = type;
        return this;
    }

    public OutboxMessageSpecification OfCreatedDateTime(DateTime dateTime)
    {
        if (CreatedDateTime.HasValue) return this;
        CreatedDateTime = dateTime;
        return this;
    }

    public OutboxMessageSpecification OfSentDateTime(DateTime dateTime)
    {
        if (SentDateTime.HasValue) return this;
        SentDateTime = dateTime;
        return this;
    }
    
    public OutboxMessageSpecification OfSentOnly(bool sentOnly)
    {
        if (SentOnly.HasValue) return this;
        SentOnly = sentOnly;
        return this;
    }
    
    public OutboxMessageSpecification OfNotSentOnly(bool notSentOnly)
    {
        if (NotSentOnly.HasValue) return this;
        NotSentOnly = notSentOnly;
        return this;
    }
    
    public OutboxMessageSpecification OfWithLock(bool withLock)
    {
        if (WithLock.HasValue) return this;
        WithLock = withLock;
        return this;
    }
}
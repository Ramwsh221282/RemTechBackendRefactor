namespace Identity.Domain.Contracts.Outbox;

public sealed class OutboxMessageSpecification
{
    public string? Type { get; private set; }
    public DateTime? CreatedDateTime { get; private set; }
    public DateTime? SentDateTime { get; private set; }
    public bool? SentOnly { get; private set; }
    public bool? NotSentOnly { get; private set; }
    public bool? WithLock { get; private set; }
    public int? Limit { get; private set; }
    public int? RetryCountLessThan { get; private set; }

    public OutboxMessageSpecification OfLimit(int limit)
    {
        if (Limit.HasValue)
            return this;
        Limit = limit;
        return this;
    }

    public OutboxMessageSpecification OfRetryCountLessThan(int retryCount)
    {
        if (RetryCountLessThan.HasValue)
            return this;
        RetryCountLessThan = retryCount;
        return this;
    }

    public OutboxMessageSpecification OfType(string type)
    {
        if (!string.IsNullOrWhiteSpace(Type))
            return this;
        Type = type;
        return this;
    }

    public OutboxMessageSpecification OfCreatedDateTime(DateTime dateTime)
    {
        if (CreatedDateTime.HasValue)
            return this;
        CreatedDateTime = dateTime;
        return this;
    }

    public OutboxMessageSpecification OfSentDateTime(DateTime dateTime)
    {
        if (SentDateTime.HasValue)
            return this;
        SentDateTime = dateTime;
        return this;
    }

    public OutboxMessageSpecification OfSentOnly()
    {
        if (SentOnly.HasValue)
            return this;
        SentOnly = true;
        return this;
    }

    public OutboxMessageSpecification OfNotSentOnly()
    {
        if (NotSentOnly.HasValue)
            return this;
        NotSentOnly = true;
        return this;
    }

    public OutboxMessageSpecification OfWithLock()
    {
        if (WithLock.HasValue)
            return this;
        WithLock = true;
        return this;
    }
}

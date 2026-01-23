namespace ParsersControl.Core.Contracts;

public sealed record SubscribedParserQuery(
    string? Domain = null,
    string? Type = null,
    Guid? Id = null,
    bool WithLock = false
)
{
    public SubscribedParserQuery WithId(Guid id)
    {
        return Id.HasValue ? this : (this with { Id = id });
    }

    public SubscribedParserQuery OfDomain(string domain)
    {
        return !string.IsNullOrWhiteSpace(Domain) ? this : (this with { Domain = domain });
    }

    public SubscribedParserQuery OfType(string type)
    {
        return !string.IsNullOrWhiteSpace(Type) ? this : (this with { Type = type });
    }

    public SubscribedParserQuery RequireLock() => this with { WithLock = true };
}

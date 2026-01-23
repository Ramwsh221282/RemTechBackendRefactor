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
		if (Id.HasValue)
			return this;
		return this with { Id = id };
	}

	public SubscribedParserQuery OfDomain(string domain)
	{
		if (!string.IsNullOrWhiteSpace(Domain))
			return this;
		return this with { Domain = domain };
	}

	public SubscribedParserQuery OfType(string type)
	{
		if (!string.IsNullOrWhiteSpace(Type))
			return this;
		return this with { Type = type };
	}

	public SubscribedParserQuery RequireLock() => this with { WithLock = true };
}

namespace ParsersControl.Core.Contracts;

public sealed record SubscribedParsersCollectionQuery(
	IEnumerable<string>? Domains = null,
	IEnumerable<string>? Types = null,
	IEnumerable<Guid>? Identifiers = null,
	bool WithLock = false
);

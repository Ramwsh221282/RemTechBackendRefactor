namespace ParsersControl.Core.Parsers.Contracts;

public sealed record SubscribedParserQuery(
    string? Domain = null,
    string? Type = null,
    Guid? Id = null,
    bool WithLock = false);
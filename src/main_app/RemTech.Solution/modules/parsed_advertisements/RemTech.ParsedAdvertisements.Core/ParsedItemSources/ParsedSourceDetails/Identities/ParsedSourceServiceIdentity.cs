using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails.Identities;

public sealed record ParsedSourceServiceIdentity
{
    private readonly NotEmptyGuid _parserId;
    private readonly NotEmptyString _parserName;

    public ParsedSourceServiceIdentity(NotEmptyGuid parserId, NotEmptyString parserName)
    {
        _parserId = parserId;
        _parserName = parserName;
    }

    public ParsedSourceServiceIdentity(Guid? parserId, string? parserName)
        : this(new NotEmptyGuid(parserId), new NotEmptyString(parserName)) { }

    public static implicit operator string(ParsedSourceServiceIdentity identity) =>
        identity._parserName;

    public static implicit operator Guid(ParsedSourceServiceIdentity identity) =>
        identity._parserId;

    public static implicit operator NotEmptyGuid(ParsedSourceServiceIdentity identity) =>
        identity._parserId;

    public static implicit operator NotEmptyString(ParsedSourceServiceIdentity identity) =>
        identity._parserName;

    public static implicit operator bool(ParsedSourceServiceIdentity identity) =>
        identity._parserId && identity._parserName;
}

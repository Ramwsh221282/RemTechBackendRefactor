using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails.Identities;

public sealed record ParsedSourceIdentity
{
    private readonly ParsedSourceServiceIdentity _parser;
    private readonly NotEmptyString _type;

    public ParsedSourceIdentity(ParsedSourceServiceIdentity parser, NotEmptyString type)
    {
        _parser = parser;
        _type = type;
    }

    public ParsedSourceIdentity(ParsedSourceServiceIdentity parser, string? type)
        : this(parser, new NotEmptyString(type)) { }

    public ParsedSourceIdentity(Guid? parserId, string? parserName, string? type)
        : this(new ParsedSourceServiceIdentity(parserId, parserName), new NotEmptyString(type)) { }

    public static implicit operator bool(ParsedSourceIdentity identity) =>
        identity._parser && identity._type;
}

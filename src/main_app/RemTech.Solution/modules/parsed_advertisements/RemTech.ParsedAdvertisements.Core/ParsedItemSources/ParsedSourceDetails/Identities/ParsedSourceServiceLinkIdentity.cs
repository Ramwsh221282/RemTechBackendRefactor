using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails.Identities;

public sealed record ParsedSourceServiceLinkIdentity
{
    private readonly NotEmptyGuid _linkId;
    private readonly NotEmptyString _linkName;
    private readonly NotEmptyString _linkUrl;

    public ParsedSourceServiceLinkIdentity(
        NotEmptyGuid linkId,
        NotEmptyString linkName,
        NotEmptyString linkUrl
    )
    {
        _linkId = linkId;
        _linkName = linkName;
        _linkUrl = linkUrl;
    }

    public ParsedSourceServiceLinkIdentity(Guid? linkId, string? linkName, string? linkUrl)
        : this(new NotEmptyGuid(linkId), new NotEmptyString(linkName), new NotEmptyString(linkUrl))
    { }

    public static implicit operator bool(ParsedSourceServiceLinkIdentity identity) =>
        identity._linkId && identity._linkName && identity._linkUrl;
}

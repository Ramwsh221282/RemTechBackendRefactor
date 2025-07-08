using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;

public sealed class ParserLinkUrl
{
    private readonly NotEmptyString _url;

    public ParserLinkUrl(NotEmptyString url) => _url = url;

    public NotEmptyString Read() => _url;

    public bool SameBy(NotEmptyString other) => _url.Same(other);

    public bool SameBy(ParserLinkUrl other) => SameBy(other._url);

    public static implicit operator string(ParserLinkUrl url) => url._url;
}

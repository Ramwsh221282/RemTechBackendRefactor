using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.Primitives.Comparing;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls.Compares;

public sealed class CompareParserLinkUrl : ICompare
{
    private readonly bool _compared;

    public CompareParserLinkUrl(NotEmptyString linkUrl, NotEmptyString other)
        : this(linkUrl.StringValue(), other.StringValue()) { }

    public CompareParserLinkUrl(IParserLink link, IParserLink other)
        : this(link.ReadUrl(), other.ReadUrl().Read()) { }

    public CompareParserLinkUrl(IParserLink link, NotEmptyString other)
        : this(link, other.StringValue()) { }

    public CompareParserLinkUrl(IParserLink link, string other)
        : this(link.ReadUrl().Read().StringValue(), other) { }

    public CompareParserLinkUrl(ParserLinkUrl linkUrl, string other)
        : this(linkUrl.Read().StringValue(), other) { }

    public CompareParserLinkUrl(string linkUrl, string other) => _compared = linkUrl == other;

    public bool Equality() => _compared;

    public static implicit operator bool(CompareParserLinkUrl compare) => compare._compared;
}

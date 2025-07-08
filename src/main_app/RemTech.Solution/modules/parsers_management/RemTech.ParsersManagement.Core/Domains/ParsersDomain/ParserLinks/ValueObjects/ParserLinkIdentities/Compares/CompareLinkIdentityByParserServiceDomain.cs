using RemTech.Core.Shared.Primitives.Comparing;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;

public sealed class CompareLinkIdentityByParserServiceDomain : ICompare
{
    private readonly bool _compare;

    public CompareLinkIdentityByParserServiceDomain(IParserLink parserLink, IParser parser)
        : this(parserLink.ReadUrl().Read(), parser.Identification().Domain()) { }

    public CompareLinkIdentityByParserServiceDomain(string linkUrl, string parserDomain) =>
        _compare = linkUrl.Contains(parserDomain);

    public bool Equality() => _compare;

    public static implicit operator bool(CompareLinkIdentityByParserServiceDomain compare) =>
        compare._compare;
}

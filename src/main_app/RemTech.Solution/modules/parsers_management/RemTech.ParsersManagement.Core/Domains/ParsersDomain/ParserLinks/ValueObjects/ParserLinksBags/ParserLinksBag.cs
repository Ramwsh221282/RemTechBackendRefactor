using RemTech.ParsersManagement.Core.Common.Extensions;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;

public sealed class LinkFromParserBag
{
    private readonly MaybeBag<IParserLink> _link;
    private ParserLinksBag _bag;

    public LinkFromParserBag(MaybeBag<IParserLink> link, ParserLinksBag bag)
    {
        _link = link;
        _bag = bag;
    }

    public bool Remove()
    {
        if (!_link.Any())
            return false;
        _bag -= _link.Take();
        return true;
    }

    public bool Any() => _link.Any();

    public IParserLink Link() => _link.Take();
}

public sealed class ParserLinksBag
{
    private readonly List<IParserLink> _links;

    public ParserLinksBag(IEnumerable<IParserLink> links) => _links = [.. links];

    public ParserLinksBag() => _links = [];

    public PositiveInteger Amount() => PositiveInteger.New(_links.Count);

    public LinkFromParserBag FindConcrete(Func<IParserLink, bool> term) =>
        new(_links.Maybe(term), this);

    public IEnumerable<IParserLink> Read() => _links;

    public static ParserLinksBag operator +(ParserLinksBag left, IParserLink link)
    {
        left._links.Add(link);
        return left;
    }

    public static ParserLinksBag operator -(ParserLinksBag left, IParserLink link)
    {
        left._links.Remove(link);
        return left;
    }
}

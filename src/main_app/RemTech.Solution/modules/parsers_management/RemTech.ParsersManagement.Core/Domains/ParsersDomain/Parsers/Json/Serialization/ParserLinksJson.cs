using RemTech.Json.Library.Serialization;
using RemTech.Json.Library.Serialization.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

public sealed class ParserLinksJson
{
    private readonly ObjectsArraySerJson<IParserLink> _json;

    public ParserLinksJson(IParser parser)
    {
        _json = new ObjectsArraySerJson<IParserLink>("links", parser.OwnedLinks().Read()).ForEach(
            l =>
                new PlainSerJson()
                    .With(new ParserLinkIdentityJson(l))
                    .With(new StringSerJson("url", l.ReadUrl()))
                    .With(new BooleanSerJson("activity", l.Activity()))
                    .With(new ParserLinkStatisticsJson(l))
        );
    }

    public ISerJson Json() => _json;
}

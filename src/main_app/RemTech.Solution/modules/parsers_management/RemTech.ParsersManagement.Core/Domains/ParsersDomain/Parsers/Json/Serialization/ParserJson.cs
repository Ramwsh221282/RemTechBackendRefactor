using RemTech.Json.Library.Serialization.Primitives;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

public sealed class ParserJson
{
    private readonly string _json;

    public ParserJson(IParser parser)
    {
        _json = new PlainSerJson()
            .With(new ParserIdentityJson(parser))
            .With(new StringSerJson("state", parser.WorkState()))
            .With(new StringSerJson("domain", parser.Domain().Read()))
            .With(new ParserWorkStatisticsJson(parser))
            .With(new ParserWorkScheduleJson(parser))
            .With(new ParserLinksJson(parser).Json())
            .Read();
    }

    public static implicit operator string(ParserJson json) => json._json;
}

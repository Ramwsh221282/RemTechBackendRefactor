using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

public sealed class ParserCacheJson
{
    private readonly ParserCacheKey _key;
    private readonly ParserJson _json;

    public ParserCacheJson(IParser parser)
    {
        _key = new ParserCacheKey(parser);
        _json = new ParserJson(parser);
    }

    public string Key() => _key;

    public string Json() => _json;
}

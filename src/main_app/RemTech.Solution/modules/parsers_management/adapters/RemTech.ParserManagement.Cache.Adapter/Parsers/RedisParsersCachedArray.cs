using System.Text.Json;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class RedisParsersCachedArray
{
    private readonly string[] _parserJsons;

    public RedisParsersCachedArray()
    {
        _parserJsons = [];
    }

    public RedisParsersCachedArray(string[] parserJsons)
    {
        _parserJsons = parserJsons;
    }

    public RedisParsersCachedArray(ParserCacheJson parserJson)
    {
        _parserJsons = [parserJson.Json()];
    }

    public RedisParsersCachedArray(RedisParsersCachedArray origin, ParserCacheJson json, int index)
    {
        string[] jsons = origin._parserJsons;
        jsons[index] = json.Json();
        _parserJsons = jsons;
    }

    public string[] Copy()
    {
        return [.. _parserJsons];
    }

    public bool Any()
    {
        return _parserJsons.Length > 0;
    }

    public string Serialized()
    {
        return JsonSerializer.Serialize(_parserJsons);
    }

    public Length Length() => new(_parserJsons.Length);
}

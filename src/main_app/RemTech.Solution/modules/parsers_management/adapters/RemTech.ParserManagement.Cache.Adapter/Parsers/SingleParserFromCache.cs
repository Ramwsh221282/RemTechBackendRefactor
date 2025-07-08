using RemTech.Json.Library.Deserialization;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class SingleParserFromCache(string jsonParser)
{
    public IParser Read()
    {
        return new DeserializedParser(new DesJsonSource(jsonParser)).Map();
    }
}

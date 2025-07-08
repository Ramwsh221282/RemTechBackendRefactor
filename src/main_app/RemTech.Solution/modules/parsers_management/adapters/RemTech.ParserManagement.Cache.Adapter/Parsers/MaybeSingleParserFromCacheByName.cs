using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class MaybeSingleParserFromCacheByName(Name name)
{
    public async Task<MaybeBag<IParser>> Read(RedisParsers parsers)
    {
        RedisParsersCachedArray array = await parsers.GetArray();
        string[] jsons = array.Copy();
        foreach (string json in jsons)
        {
            IParser parser = new SingleParserFromCache(json).Read();
            if (name.Equals(parser.Identification().ReadName()))
                return new MaybeBag<IParser>(parser);
        }
        return new MaybeBag<IParser>();
    }
}

public sealed class MaybeSingleParserFromCacheByTypeAndDomain(
    ParsingType type,
    NotEmptyString domain
)
{
    private readonly ParserServiceDomain _domain = new(domain);

    public async Task<MaybeBag<IParser>> Read(RedisParsers parsers)
    {
        RedisParsersCachedArray array = await parsers.GetArray();
        string[] jsons = array.Copy();
        foreach (string json in jsons)
        {
            IParser parser = new SingleParserFromCache(json).Read();
            ParserIdentity identification = parser.Identification();
            if (type.Equals(identification.ReadType()) && _domain.Equals(identification.Domain()))
                return new MaybeBag<IParser>(parser);
        }

        return new MaybeBag<IParser>();
    }
}

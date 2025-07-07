using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class MaybeSingleParserFromCache(RedisParsersCachedArray array, ParserCacheKey key)
{
    public MaybeBag<IParser> Read()
    {
        string[] jsons = array.Copy();
        foreach (string json in jsons)
        {
            IParser readed = new SingleParserFromCache(json).Read();
            Guid id = Guid.Parse(key);
            Guid readedid = readed.Identification().ReadId();
            if (id == readedid)
                return new MaybeBag<IParser>(readed);
        }
        return new MaybeBag<IParser>();
    }
}

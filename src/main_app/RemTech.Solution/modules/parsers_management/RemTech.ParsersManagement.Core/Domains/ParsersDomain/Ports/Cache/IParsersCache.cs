using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

public interface IParsersCache
{
    public Task Invalidate(ParserCacheJson json);
    public Task<MaybeBag<IParser>> Get(ParserCacheKey key);
    public Task<IParser[]> Get();
}

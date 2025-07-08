using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

public interface IParsersCache
{
    public Task Invalidate(ParserCacheJson json);
    public Task<MaybeBag<IParser>> Get(ParserCacheKey key);
    public Task<MaybeBag<IParser>> Get(Name name);
    public Task<MaybeBag<IParser>> Get(ParsingType type, NotEmptyString domain);
    public Task<IParser[]> Get();
}

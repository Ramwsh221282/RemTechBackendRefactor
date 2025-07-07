using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

public sealed class ParserCacheKey
{
    private readonly string _key;

    public ParserCacheKey(IParser parser)
    {
        _key = parser.Identification().ReadId();
    }

    public ParserCacheKey(Guid id)
    {
        _key = id.ToString();
    }

    public static implicit operator string(ParserCacheKey key) => key._key;
}
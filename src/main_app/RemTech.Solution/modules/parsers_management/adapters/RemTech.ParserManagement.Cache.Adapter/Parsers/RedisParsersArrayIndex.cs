using RemTech.Json.Library.Deserialization;
using RemTech.Json.Library.Deserialization.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class RedisParsersArrayIndex(RedisParsersCachedArray array, ParserCacheJson json)
{
    private int _index;

    public int Index()
    {
        using DesJsonSource relatedSource = new(json.Json());
        Guid relatedId = new DesJsonGuid(relatedSource["id"]);
        string[] jsons = array.Copy();
        for (int index = 0; index < jsons.Length; index++)
        {
            string entry = jsons[index];
            using DesJsonSource entrySource = new(entry);
            Guid entryId = new DesJsonGuid(entrySource["id"]);
            if (relatedId == entryId)
            {
                _index = index;
                break;
            }
        }

        _index = -1;
        return _index;
    }
}

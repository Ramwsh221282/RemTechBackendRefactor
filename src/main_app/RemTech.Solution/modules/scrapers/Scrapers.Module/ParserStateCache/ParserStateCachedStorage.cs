using StackExchange.Redis;

namespace Scrapers.Module.ParserStateCache;

internal sealed class ParserStateCachedStorage(ConnectionMultiplexer multiplexer)
{
    public async Task UpdateState(string name, string type, string state)
    {
        IDatabase db = multiplexer.GetDatabase();
        string key = MakeKey(name, type);
        await db.StringSetAsync(key, state);
    }

    private static string MakeKey(string name, string type)
    {
        string key = $"state_{name}_{type}";
        return key;
    }
}

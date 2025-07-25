namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class RedisDeletedParsersArray(string key)
{
    public async Task<RedisParsersCachedArray> Invalidate(RedisCacheEngine engine)
    {
        RedisParsersCachedArray array = new();
        bool deletion = await engine.Access().KeyDeleteAsync(key);
        return array;
    }
}

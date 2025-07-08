using System.Text.Json;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class MaybeRedisParsersCachedArray(string key)
{
    public async Task<RedisParsersCachedArray> Read(RedisCacheEngine engine)
    {
        string? arrayJson = await engine.Access().StringGetAsync(key);
        if (string.IsNullOrEmpty(arrayJson))
            return new RedisParsersCachedArray();
        string[]? arrayParts = JsonSerializer.Deserialize<string[]>(arrayJson);
        return arrayParts == null
            ? new RedisParsersCachedArray()
            : new RedisParsersCachedArray(arrayParts);
    }
}
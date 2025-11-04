using Mailing.Domain.Postmans;
using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Mailing.Adapters.Cache.Postmans;

public sealed class PostmansCache(RedisCache cache)
{
    private const string KeyTemplate = "POSTMAN_{0}";
    private readonly IDatabase _database = cache.Database;

    public async Task Add(IPostman postman)
    {
        TimeSpan timeSpan = TimeSpan.FromMinutes(5);
        await _database.StringSetAsync(CreateKey(postman), Serialize(postman), timeSpan);
    }

    public async Task<bool> Contains(IPostman postman)
    {
        string? json = await _database.StringGetAsync(CreateKey(postman));
        return json != null;
    }

    private string CreateKey(IPostman postman) =>
        string.Format(KeyTemplate, postman.Data.Email);

    private string Serialize(IPostman postman) =>
        new JsonPostman(postman: postman).Serialize();

    private IPostman Deserialize(string json) =>
        new JsonPostman(json).Deserialize();
}
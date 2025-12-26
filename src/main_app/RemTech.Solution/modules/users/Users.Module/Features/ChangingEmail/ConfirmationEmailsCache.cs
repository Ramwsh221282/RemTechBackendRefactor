using Shared.Infrastructure.Module.Redis;

namespace Users.Module.Features.ChangingEmail;

internal sealed class ConfirmationEmailsCache(RedisCache cache)
{
    private const string Key = "confirm_email_{0}";

    public async Task Create(Guid keyId, Guid userId)
    {
        string key = MakeKey(keyId);
        string value = userId.ToString();
        await cache.Database.StringSetAsync(key, value, TimeSpan.FromDays(1));
    }

    public async Task<Guid> Confirm(Guid keyId)
    {
        string key = MakeKey(keyId);
        string? value = await cache.Database.StringGetAsync(key);
        return string.IsNullOrWhiteSpace(value)
            ? throw new ConfirmationEmailExpiredException()
            : Guid.Parse(value);
    }

    public async Task Delete(Guid keyId)
    {
        string key = MakeKey(keyId);
        await cache.Database.KeyDeleteAsync(key);
    }

    private string MakeKey(Guid keyId)
    {
        string keyString = keyId.ToString();
        return string.Format(Key, keyString);
    }
}

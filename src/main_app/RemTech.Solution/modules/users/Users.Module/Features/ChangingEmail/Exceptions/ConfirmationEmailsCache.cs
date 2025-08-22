using StackExchange.Redis;

namespace Users.Module.Features.ChangingEmail.Exceptions;

internal sealed class ConfirmationEmailsCache(ConnectionMultiplexer multiplexer)
{
    private const string Key = "confirm_email_{0}";

    public async Task Create(Guid keyId, Guid userId)
    {
        string key = MakeKey(keyId);
        string value = userId.ToString();
        IDatabase db = multiplexer.GetDatabase();
        await db.StringSetAsync(key, value, TimeSpan.FromDays(1));
    }

    public async Task<Guid> Confirm(Guid keyId)
    {
        string key = MakeKey(keyId);
        IDatabase db = multiplexer.GetDatabase();
        string? value = await db.StringGetAsync(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new ConfirmationEmailExpiredException();
        return Guid.Parse(value);
    }

    public async Task Delete(Guid keyId)
    {
        string key = MakeKey(keyId);
        IDatabase db = multiplexer.GetDatabase();
        await db.KeyDeleteAsync(key);
    }

    private string MakeKey(Guid keyId)
    {
        string keyString = keyId.ToString();
        return string.Format(Key, keyString);
    }
}

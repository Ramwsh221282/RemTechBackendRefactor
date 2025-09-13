using StackExchange.Redis;
using Users.Module.Features.UserPasswordRecovering.Core;

namespace Users.Module.Features.UserPasswordRecovering.Infrastructure;

internal sealed class PasswordRecoveringCache
{
    private const string KeyTemplate = "USER_PASSWORD_RESET_{0}";
    private readonly IDatabase _database;

    public PasswordRecoveringCache(ConnectionMultiplexer multiplexer)
    {
        _database = multiplexer.GetDatabase();
    }

    public async Task<Guid> GenerateRecoveringKey(UserToRecover user)
    {
        string key = GenerateKey();
        await PutKey(key, user);
        return ParseKeyGuid(key);
    }

    public async Task<Guid> ReceiveUserId(string key)
    {
        string? userIdString = await _database.StringGetAsync(ApplyKeyTemplate(key));
        return string.IsNullOrWhiteSpace(userIdString)
            ? throw new RecoveryPasswordKeyValueNotFoundException()
            : Guid.Parse(userIdString);
    }

    public async Task RemoveKey(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    private string ApplyKeyTemplate(string key)
    {
        return string.Format(KeyTemplate, key);
    }

    private string GenerateKey()
    {
        Guid key = Guid.NewGuid();
        string keyAsString = key.ToString();
        return ApplyKeyTemplate(keyAsString);
    }

    private Guid ParseKeyGuid(string key)
    {
        return Guid.Parse(key.Split('_')[^1]);
    }

    private async Task PutKey(string key, UserToRecover user)
    {
        user.Print(out Guid id, out _, out _);
        string idString = id.ToString();
        await _database.StringSetAsync(key, idString, TimeSpan.FromMinutes(10));
    }
}

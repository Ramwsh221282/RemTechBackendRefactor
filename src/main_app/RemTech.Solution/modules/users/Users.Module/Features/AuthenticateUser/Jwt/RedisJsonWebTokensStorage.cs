using System.Text.Json;
using StackExchange.Redis;

namespace Users.Module.Features.AuthenticateUser.Jwt;

internal sealed class RedisJsonWebTokensStorage(ConnectionMultiplexer multiplexer)
    : IJsonWebTokensStorage
{
    public async Task SaveTokenClaimsData(
        JsonWebTokenClaimsDataOutput output,
        CancellationToken ct = default
    )
    {
        IDatabase database = multiplexer.GetDatabase();
        string key = CreateStoringTokenId(output);
        string payload = CreateStoringPayload(output);
        await database.StringSetAsync(key, payload, TimeSpan.FromDays(7));
    }

    private static string CreateStoringTokenId(JsonWebTokenClaimsDataOutput output)
    {
        string accessTokenId = $"access_token_{output.AccessTokenId.ToString()}";
        return accessTokenId;
    }

    private static string CreateStoringPayload(JsonWebTokenClaimsDataOutput output)
    {
        string json = JsonSerializer.Serialize(output);
        return json;
    }
}

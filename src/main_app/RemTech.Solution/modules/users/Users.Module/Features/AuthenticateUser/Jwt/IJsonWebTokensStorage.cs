namespace Users.Module.Features.AuthenticateUser.Jwt;

internal interface IJsonWebTokensStorage
{
    Task SaveTokenClaimsData(JsonWebTokenClaimsDataOutput output, CancellationToken ct = default);
}

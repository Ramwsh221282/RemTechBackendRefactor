namespace Users.Module.Features.AuthenticateUser.Jwt;

internal sealed record JsonWebTokenClaimsDataOutput(
    Guid AccessTokenId,
    string RefreshToken,
    Guid UserId
);

namespace Identity.Domain.Accounts.Features.Authenticate;

public sealed record AuthenticationResult(string AccessToken, string RefreshToken);
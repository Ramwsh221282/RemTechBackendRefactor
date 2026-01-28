namespace Identity.Domain.Accounts.Features.Authenticate;

/// <summary>
/// Результат аутентификации пользователя.
/// </summary>
/// <param name="AccessToken">Токен доступа.</param>
/// <param name="RefreshToken">Токен обновления.</param>
public sealed record AuthenticationResult(string AccessToken, string RefreshToken);

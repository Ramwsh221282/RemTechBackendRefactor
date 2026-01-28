namespace Identity.Infrastructure.Accounts.Queries.GetUser;

/// <summary>
/// Запрос на получение пользователя по рефреш-токену.
/// </summary>
/// <param name="RefreshToken">Рефреш-токен пользователя.</param>
public sealed record GetUserByRefreshTokenQuery(string RefreshToken) : GetUserQuery;

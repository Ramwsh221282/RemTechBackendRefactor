namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed record GetUserByRefreshTokenQuery(string RefreshToken) : GetUserQuery;

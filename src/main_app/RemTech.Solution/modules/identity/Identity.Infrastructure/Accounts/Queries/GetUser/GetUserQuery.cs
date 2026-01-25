using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public record GetUserQuery : IQuery;

public sealed record GetUserByIdQuery(Guid AccountId) : GetUserQuery;

public sealed record GetUserByRefreshTokenQuery(string RefreshToken) : GetUserQuery;

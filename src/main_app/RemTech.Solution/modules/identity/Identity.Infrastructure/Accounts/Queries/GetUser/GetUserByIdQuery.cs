namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed record GetUserByIdQuery(Guid AccountId) : GetUserQuery;

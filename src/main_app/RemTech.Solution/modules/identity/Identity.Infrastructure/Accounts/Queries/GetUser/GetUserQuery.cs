using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed record GetUserQuery(Guid AccountId) : IQuery;
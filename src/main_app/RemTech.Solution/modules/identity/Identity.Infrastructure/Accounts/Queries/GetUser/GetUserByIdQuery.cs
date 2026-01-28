namespace Identity.Infrastructure.Accounts.Queries.GetUser;

/// <summary>
/// Запрос на получение пользователя по идентификатору.
/// </summary>
/// <param name="AccountId">Идентификатор пользователя.</param>
public sealed record GetUserByIdQuery(Guid AccountId) : GetUserQuery;

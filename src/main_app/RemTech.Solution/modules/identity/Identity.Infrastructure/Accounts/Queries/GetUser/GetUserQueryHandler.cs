using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed class GetUserQueryHandler(IAccountsRepository accounts)
    : IQueryHandler<GetUserQuery, UserAccountResponse?>
{
    private IAccountsRepository Accounts { get; } = accounts;

    public async Task<UserAccountResponse?> Handle(GetUserQuery query, CancellationToken ct = default) =>
        query switch
        {
            GetUserByIdQuery byId => await SearchById(byId, ct),
            GetUserByRefreshTokenQuery byToken => await SearchById(byToken, ct),
            _ => null,
        };

    private async Task<UserAccountResponse?> SearchById(GetUserByRefreshTokenQuery query, CancellationToken ct) =>
        CreateResponse(await Accounts.Find(new AccountSpecification().WithRefreshToken(query.RefreshToken), ct));

    private async Task<UserAccountResponse?> SearchById(GetUserByIdQuery query, CancellationToken ct) =>
        CreateResponse(await Accounts.Find(new AccountSpecification().WithId(query.AccountId), ct));

    private static UserAccountResponse? CreateResponse(Result<Account> result) =>
        result.IsFailure ? null : UserAccountResponse.Create(result.Value);
}

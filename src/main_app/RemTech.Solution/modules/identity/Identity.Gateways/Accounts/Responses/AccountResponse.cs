using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.Responses;

public sealed record AccountResponse(
    Guid Id,
    string Name,
    string Email,
    bool Activated) : IResponse
{
    public static AccountResponse Represent(IAccount account)
    {
        AccountData data = account.Represent();
        return new AccountResponse(data.Id, data.Name, data.Email, data.Activated);
    }
}
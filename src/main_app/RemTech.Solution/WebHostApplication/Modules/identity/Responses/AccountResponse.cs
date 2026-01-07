using Identity.Domain.Accounts.Models;

namespace WebHostApplication.Modules.identity.Responses;

public sealed record AccountResponse(
    Guid Id,
    string Login,
    string Email,
    bool IsActivated,
    IEnumerable<AccountPermissionsResponse> Permissions)
{
    public static AccountResponse ConvertFrom(Account account) =>
        new(account.Id.Value,
            account.Login.Value,
            account.Email.Value,
            account.ActivationStatus.Value,
            account.PermissionsList.Select(p => AccountPermissionsResponse.ConvertFrom(p)));
}
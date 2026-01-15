using Identity.Domain.Accounts.Models;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed record UserAccountResponse(
    Guid Id,
    string Login,
    string Email,
    bool IsActivated,
    IEnumerable<UserAccountPermissionResponse> Permissions
)
{
    public static UserAccountResponse Create(Account account)
    {
        return new UserAccountResponse(
            account.Id.Value,
            account.Login.Value,
            account.Email.Value,
            account.ActivationStatus.Value,
            account.PermissionsList.Select(p => UserAccountPermissionResponse.Create(p))
        );
    }
}

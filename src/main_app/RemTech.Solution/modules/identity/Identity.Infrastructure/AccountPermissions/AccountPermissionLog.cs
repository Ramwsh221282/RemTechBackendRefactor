using Identity.Contracts.AccountPermissions.Contracts;

namespace Identity.Infrastructure.AccountPermissions;

public sealed class AccountPermissionLog
{
    private Guid _accountId = Guid.Empty;
    private Guid _permissionId = Guid.Empty;
    private string _email = string.Empty;
    private string _accountName = string.Empty;
    private string _permissionName = string.Empty;
    private void AddAccountId(Guid id) => _accountId = id;
    private void AddPermissionId(Guid id) => _permissionId = id;
    private void AddEmail(string email) => _email = email;
    private void AddAccountName(string accountName) => _accountName = accountName;
    private void AddPermissionName(string permissionName) => _permissionName = permissionName;

    public void Log(Serilog.ILogger logger)
    {
        object[] logProperties = [_accountId, _permissionId, _email, _accountName, _permissionName];
        logger.Information(
            """
            Account permission info:
            Permission Id: {Id}
            Account Id: {AccountId}
            Email: {Email}
            Account Name: {AccountName}
            Permission Name: {PermissionName}
            """,
            logProperties);
    }

    public AccountPermissionLog(IAccountPermission accountPermission)
    {
        accountPermission.Write(
            AddPermissionId, 
            AddAccountId, 
            AddAccountName, 
            AddEmail, 
            AddPermissionName);
    }
}
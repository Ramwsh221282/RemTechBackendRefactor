using Identity.Application.AccountPermissions;
using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.AccountPermissions.Shared;

public sealed class AccountPermissionResponse : 
    IResponse, 
    IOnAccountPermissionCreatedEventListener,
    IOnAccountPermissionRemovedEventListener
{
    public Guid AccountId { get; private set; } = Guid.Empty;
    public Guid PermissionId { get; private set; } = Guid.Empty;
    public string AccountName { get; private set; } = string.Empty;
    public string PermissionName { get; private set; } = string.Empty;
    public string AccountEmail { get; private set; } = string.Empty;
    private void AddAccountId(Guid accountId) =>  AccountId = accountId;
    private void AddPermissionId(Guid permissionId) =>  PermissionId = permissionId;
    private void AddAccountName(string accountName) =>  AccountName = accountName;
    private void AddPermissionName(string permissionName) =>  PermissionName = permissionName;
    private void AddAccountEmail(string accountEmail) =>  AccountEmail = accountEmail;
    
    public Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default)
    {
        new AccountPermission(data).Write(
            AddPermissionId,
            AddAccountId,
            AddAccountName,
            AddAccountEmail,
            AddPermissionName
        );
        return Task.FromResult(Result.Success(Unit.Value));
    }
}
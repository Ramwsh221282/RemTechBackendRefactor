using Identity.Gateways.AccountPermissions.Shared;
using Identity.Gateways.Accounts.Responses;
using Identity.Gateways.Permissions.AddPermission;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.AccountPermissions.Fixture;
using Tests.Identity.CommonFeatures.AccountPermissions;
using Tests.Identity.CommonFeatures.Accounts;
using Tests.Identity.CommonFeatures.Permissions;

namespace Tests.Identity.AccountPermissions.Tests;

public sealed class AddAccountPermissionTests(AccountPermissionsFixture fixture) : IClassFixture<AccountPermissionsFixture>
{
    [Fact]
    private async Task Add_Account_Permission_Success()
    {
        const string email = "accountEmail@mail.com";
        const string name = "accountName";
        const string password = "SomePassword!23";
        const string permissionName = "create.account";

        Result<AccountResponse> account = await new AddAccount(fixture.Services).Invoke(name, email, password);
        Assert.True(account.IsSuccess);
        Result<AddPermissionResponse> permission = await new AddPermission(fixture.Services).Invoke(permissionName);
        Assert.True(permission.IsSuccess);
        Guid accountId = account.Value.Id;
        Guid permissionId = permission.Value.Id;
        Result<AccountPermissionResponse> attachment = await new AttachPermissionForAccount(fixture.Services)
            .Invoke(accountId, permissionId);
        Assert.True(attachment.IsSuccess);
    }

    [Fact]
    private async Task Add_Account_Permission_Non_Existent_Account()
    {
        const string permissionName = "create.account";
        Result<AddPermissionResponse> permission = await new AddPermission(fixture.Services).Invoke(permissionName);
        Assert.True(permission.IsSuccess);
        Guid permissionId = permission.Value.Id;
        Guid accountId = Guid.NewGuid();
        Result<AccountPermissionResponse> attachment = await new AttachPermissionForAccount(fixture.Services)
            .Invoke(accountId, permissionId);
        Assert.True(attachment.IsFailure);
    }

    [Fact]
    private async Task Add_Account_Permission_Non_Existent_Permission()
    {
        const string email = "accountEmail@mail.com";
        const string name = "accountName";
        const string password = "SomePassword!23";
        
        Result<AccountResponse> account = await new AddAccount(fixture.Services).Invoke(name, email, password);
        Assert.True(account.IsSuccess);
        Guid accountId = account.Value.Id;
        Guid permissionId = Guid.NewGuid();
        Result<AccountPermissionResponse> attachment = await new AttachPermissionForAccount(fixture.Services)
            .Invoke(accountId, permissionId);
        Assert.True(attachment.IsFailure);
    }
}
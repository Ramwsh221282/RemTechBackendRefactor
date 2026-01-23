using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

public sealed class GivePermissionsTests(IntegrationalTestsFactory factory) : IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; } = factory.Services;

	[Fact]
	private async Task Give_Account_Permissions_Success()
	{
		const string accountName = "test name";
		const string accountEmail = "testEmail@mail.com";
		const string accountPassword = "simplePassword@123";
		Permission[] permissions = (await Services.GetPermissions()).ToArray();

		Result<Unit> account = await Services.InvokeAccountRegistration(
			new RegisterAccountCommand(accountEmail, accountName, accountPassword)
		);
		Assert.True(account.IsSuccess);

		Result<Account> registered = await Services.GetAccountByEmail(accountEmail);
		Assert.True(registered.IsSuccess);

		Result<Account> withPermissions = await Services.GivePermissions(
			registered.Value.Id.Value,
			permissions.Select(p => p.Id.Value)
		);
		Assert.True(withPermissions.IsSuccess);

		Result<Account> updated = await Services.GetAccountByEmail(accountEmail);
		Assert.True(updated.IsSuccess);
		Assert.NotEmpty(updated.Value.PermissionsList);
		Assert.Equal(permissions.Length, updated.Value.PermissionsCount);
	}
}

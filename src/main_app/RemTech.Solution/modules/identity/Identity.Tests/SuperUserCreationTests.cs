using Identity.Domain.Accounts.Models;
using Identity.Domain.Permissions;
using Identity.WebApi.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

/// <summary>
/// Тесты создания суперпользователя.
/// </summary>
/// <param name="factory">Фабрика для интеграционных тестов.</param>
public sealed class SuperUserCreationTests(IntegrationalTestsFactory factory) : IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; } = factory.Services;

	[Fact]
	private async Task Ensure_Super_User_Created()
	{
		await Task.Delay(TimeSpan.FromSeconds(15));
		SuperUserCredentialsOptions options = GetOptions();
		Result<Account> account = await GetSuperUserAccount(options);
		Assert.True(account.IsSuccess);

		Permission[] permissions = await GetPermissions();
		Assert.NotEmpty(account.Value.PermissionsList);
		Assert.Equal(permissions.Length, account.Value.PermissionsCount);
	}

	private SuperUserCredentialsOptions GetOptions() =>
		Services.GetRequiredService<IOptions<SuperUserCredentialsOptions>>().Value;

	private async Task<Permission[]> GetPermissions()
	{
		IEnumerable<Permission> permissions = await Services.GetPermissions();
		return permissions.ToArray();
	}

	private Task<Result<Account>> GetSuperUserAccount(SuperUserCredentialsOptions options) =>
		Services.GetAccountByName(options.Login);
}

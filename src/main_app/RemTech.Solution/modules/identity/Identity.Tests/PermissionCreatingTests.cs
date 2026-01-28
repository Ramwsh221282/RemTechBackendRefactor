using Identity.Domain.Permissions;
using Identity.Domain.Permissions.Features.AddPermissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

/// <summary>
/// Тесты создания разрешений.
/// </summary>
/// <param name="factory">Фабрика для интеграционных тестов.</param>
public sealed class PermissionCreatingTests(IntegrationalTestsFactory factory)
	: IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; } = factory.Services;

	[Fact]
	private async Task Add_Permissions_List_Success()
	{
		IEnumerable<AddPermissionCommandPayload> payload =
		[
			new("permission.first", "Test first permission"),
			new("permission.second", "Test second permission"),
			new("permission.third", "Test third permission"),
			new("permission.fourth", "Test fourth permission"),
		];

		Result<IEnumerable<Permission>> result = await Services.AddPermissions(payload);
		Assert.True(result.IsSuccess);
	}

	[Fact]
	private async Task Add_Permissions_List_With_Duplicates_Failure()
	{
		IEnumerable<AddPermissionCommandPayload> payload1 =
		[
			new("permission.first", "Test first permission"),
			new("permission.second", "Test second permission"),
			new("permission.third", "Test third permission"),
			new("permission.fourth", "Test fourth permission"),
		];

		Result<IEnumerable<Permission>> result1 = await Services.AddPermissions(payload1);
		Assert.True(result1.IsSuccess);

		IEnumerable<AddPermissionCommandPayload> payload2 =
		[
			new("permission.third", "Test third permission"),
			new("permission.fourth", "Test fourth permission"),
			new("permission.fifth", "Test fifth permission"),
			new("permission.sixth", "Test sixth permission"),
		];

		Result<IEnumerable<Permission>> result2 = await Services.AddPermissions(payload2);
		Assert.True(result2.IsFailure);
	}

	[Fact]
	private async Task Invoke_Registration_Of_PreDefined_Permissions_Duplicated_Failure()
	{
		IEnumerable<AddPermissionCommandPayload> payload =
		[
			new(
				PredefinedPermissions.NotificationsManagementName.Value,
				PredefinedPermissions.NotificationsManagementDescription.Value
			),
			new(
				PredefinedPermissions.IdentityManagementName.Value,
				PredefinedPermissions.IdentityManagementDescription.Value
			),
			new(
				PredefinedPermissions.ParserManagementName.Value,
				PredefinedPermissions.ParserManagementDescription.Value
			),
			new(
				PredefinedPermissions.WatchItemSourcesName.Value,
				PredefinedPermissions.WatchItemSourcesDescription.Value
			),
			new(
				PredefinedPermissions.AddItemsToFavoritesName.Value,
				PredefinedPermissions.AddItemsToFavoritesDescription.Value
			),
			new(
				PredefinedPermissions.AccessTelemetryName.Value,
				PredefinedPermissions.AccessTelemetryDescription.Value
			),
		];
		Result<IEnumerable<Permission>> result = await Services.AddPermissions(payload);
		Assert.True(result.IsFailure);
	}
}

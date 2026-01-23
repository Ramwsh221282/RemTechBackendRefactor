using Identity.Domain.Accounts.Features.GivePermissions;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Permissions;
using Identity.WebApi.Options;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.WebApi.BackgroundServices;

public sealed class SuperUserAccountPermissionsUpdateBackgroundServices(
	IServiceProvider services,
	Serilog.ILogger logger
) : BackgroundService
{
	private IServiceProvider Services { get; } = services;
	private Serilog.ILogger Logger { get; } = logger.ForContext<SuperUserAccountPermissionsUpdateBackgroundServices>();

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Execute(stoppingToken);
			await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
		}
	}

	private async Task Execute(CancellationToken ct)
	{
		try
		{
			await using AsyncServiceScope scope = Services.CreateAsyncScope();
			SuperUserCredentialsOptions options = GetOptions(scope);
			Result<Account> account = await GetSuperUserAccount(scope, ct);
			if (account.IsFailure)
			{
				Logger.Warning("Super user account {Login} {Email} not found", options.Login, options.Email);
				return;
			}

			IEnumerable<Permission> permissions = [.. await GetAllPermissions(scope, ct)];
			Permission[] permissionsToAdd = GetPermissionsToAdd(account.Value, permissions);
			if (permissionsToAdd.Length == 0)
			{
				Logger.Debug(
					"Super user account {Login} {Email} already has all permissions",
					options.Login,
					options.Email
				);
				return;
			}

			Result result = await UpdateSuperUserAccountPermissions(scope, account.Value, permissionsToAdd, ct);
			if (result.IsFailure)
			{
				Logger.Fatal(result.Error, "Error updating super user account permissions");
				return;
			}

			Logger.Information("Account permissions updated successfully.");
		}
		catch (Exception e)
		{
			Logger.Fatal(e, "Error updating super user account permissions.");
		}
	}

	private static async Task<Result> UpdateSuperUserAccountPermissions(
		AsyncServiceScope scope,
		Account account,
		Permission[] permissions,
		CancellationToken ct
	)
	{
		IEnumerable<GivePermissionsPermissionsPayload> toAdd = permissions.Select(
			p => new GivePermissionsPermissionsPayload(p.Id.Value)
		);
		GivePermissionsCommand command = new(account.Id.Value, toAdd);
		return await scope
			.ServiceProvider.GetRequiredService<ICommandHandler<GivePermissionsCommand, Account>>()
			.Execute(command, ct);
	}

	private static Permission[] GetPermissionsToAdd(Account account, IEnumerable<Permission> permissions)
	{
		return [.. permissions.ExceptBy(account.PermissionsList.Select(p => p.Id.Value), p => p.Id.Value)];
	}

	private static async Task<IEnumerable<Permission>> GetAllPermissions(AsyncServiceScope scope, CancellationToken ct)
	{
		IPermissionsRepository permissionsRepository =
			scope.ServiceProvider.GetRequiredService<IPermissionsRepository>();
		return await permissionsRepository.GetMany([], ct);
	}

	private static SuperUserCredentialsOptions GetOptions(AsyncServiceScope scope)
	{
		SuperUserCredentialsOptions options = scope
			.ServiceProvider.GetRequiredService<IOptions<SuperUserCredentialsOptions>>()
			.Value;
		options.Validate();
		return options;
	}

	private static async Task<Result<Account>> GetSuperUserAccount(AsyncServiceScope scope, CancellationToken ct)
	{
		IAccountsRepository accountsRepository = scope.ServiceProvider.GetRequiredService<IAccountsRepository>();
		AccountSpecification specification = new AccountSpecification().WithLogin(GetOptions(scope).Login);
		return await accountsRepository.Find(specification, ct);
	}
}

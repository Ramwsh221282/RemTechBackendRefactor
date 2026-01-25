using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.GivePermissions;

[TransactionalHandler]
public sealed class GivePermissionsHandler(
	IAccountsRepository accounts,
	IPermissionsRepository permissions,
	IAccountsModuleUnitOfWork unitOfWork
) : ICommandHandler<GivePermissionsCommand, Account>
{
	private IAccountsRepository Accounts { get; } = accounts;
	private IPermissionsRepository Permissions { get; } = permissions;
	private IAccountsModuleUnitOfWork UnitOfWork { get; } = unitOfWork;

	public async Task<Result<Account>> Execute(GivePermissionsCommand command, CancellationToken ct = default)
	{
		Result<Account> account = await GetRequiredAccount(command, ct);
		if (account.IsFailure)
			return account.Error;

		IEnumerable<Permission> queriedPermissions = await GetRequiredPermissions(command, ct);
		if (!PermissionsFound(queriedPermissions, command, out Error error))
			return error;

		Result<Unit> add = account.Value.AddPermissions(queriedPermissions);
		if (add.IsFailure)
			return add.Error;

		await UnitOfWork.Save(account.Value, ct);
		return account.Value;
	}

	private static bool PermissionsFound(
		IEnumerable<Permission> permissions,
		GivePermissionsCommand command,
		out Error error
	)
	{
		error = Error.NoError();
		IEnumerable<Guid> notFoundPermissions = command
			.Permissions.Select(p => p.Id)
			.ExceptBy(permissions.Select(fp => fp.Id.Value), fp => fp);
		if (notFoundPermissions.Any())
		{
			string message = $"Разрешения не найдены: {string.Join(", ", notFoundPermissions.ToString())}";
			error = Error.NotFound(message);
			return false;
		}
		return true;
	}

	private Task<IEnumerable<Permission>> GetRequiredPermissions(GivePermissionsCommand command, CancellationToken ct)
	{
		IEnumerable<PermissionSpecification> specs = command.Permissions.Select(p =>
			new PermissionSpecification().WithId(p.Id).WithLock()
		);
		return Permissions.GetMany(specs, ct);
	}

	private Task<Result<Account>> GetRequiredAccount(GivePermissionsCommand command, CancellationToken ct)
	{
		AccountSpecification spec = new AccountSpecification().WithId(command.AccountId).WithLock();
		return Accounts.Find(spec, ct);
	}
}

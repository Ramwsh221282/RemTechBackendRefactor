using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Dev_ChangePassword;

public sealed class Dev_ChangePasswordHandler(
	IAccountsRepository accounts,
	IEnumerable<IAccountPasswordRequirement> requirements,
	IPasswordHasher hasher,
	IAccountsModuleUnitOfWork unitOfWork
) : ICommandHandler<Dev_ChangePasswordCommand, Unit>
{
	public async Task<Result<Unit>> Execute(
		Dev_ChangePasswordCommand command,
		CancellationToken ct = new CancellationToken()
	)
	{
		Result<Account> account = await ResolveAccount(command, ct);
		if (account.IsFailure)
			return account.Error;
		AccountPassword password = AccountPassword.Create(command.NewPassword);
		Result<Unit> change = account.Value.ChangePassword(password, hasher, requirements);
		if (change.IsFailure)
			return change.Error;
		await unitOfWork.Save(account.Value, ct);
		return change;
	}

	private async Task<Result<Account>> ResolveAccount(Dev_ChangePasswordCommand command, CancellationToken ct)
	{
		if (command.AccountId.HasValue)
			return await accounts.Find(new AccountSpecification().WithId(command.AccountId.Value), ct: ct);
		if (!string.IsNullOrWhiteSpace(command.AccountLogin))
			return await accounts.Find(new AccountSpecification().WithLogin(command.AccountLogin), ct: ct);
		if (!string.IsNullOrWhiteSpace(command.AccountEmail))
			return await accounts.Find(new AccountSpecification().WithEmail(command.AccountEmail), ct: ct);
		return Error.Validation("Account not specified.");
	}
}

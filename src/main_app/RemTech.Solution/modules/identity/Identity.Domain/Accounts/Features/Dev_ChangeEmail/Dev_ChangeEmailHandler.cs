using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.Dev_ChangeEmail;

[TransactionalHandler]
public sealed class Dev_ChangeEmailHandler(IAccountsRepository repository, IAccountsModuleUnitOfWork unitOfWork)
	: ICommandHandler<Dev_ChangeEmailCommand, Unit>
{
	public async Task<Result<Unit>> Execute(Dev_ChangeEmailCommand command, CancellationToken ct = default)
	{
		Result<Account> account = await ResolveAccount(command, ct);
		Result<Unit> result = ChangeEmail(command, account);
		return await SaveChanges(account, result, ct);
	}

	private static Result<Unit> ChangeEmail(Dev_ChangeEmailCommand command, Result<Account> account)
	{
		if (account.IsFailure)
			return account.Error;
		account.Value.ChangeEmail(AccountEmail.Create(command.NewEmail));
		return Unit.Value;
	}

	private async Task<Result<Unit>> SaveChanges(Result<Account> account, Result<Unit> change, CancellationToken ct)
	{
		async Task<Result<Unit>> Saving()
		{
			await unitOfWork.Save(account.Value, ct);
			return Unit.Value;
		}

		Error ErrorClause(Result<Account> account, Result<Unit> change) =>
			Error.Conflict(string.Join(", ", account.Error.Message, change.Error.Message));

		return (account, change) switch
		{
			{ account.IsFailure: true, change.IsFailure: false } => account.Error,
			{ account.IsFailure: false, change.IsFailure: true } => change.Error,
			{ account.IsFailure: true, change.IsFailure: true } => ErrorClause(account, change),
			_ => await Saving(),
		};
	}

	private async Task<Result<Account>> ResolveAccount(Dev_ChangeEmailCommand command, CancellationToken ct) =>
		command switch
		{
			{ AccountId: not null } => await repository.Find(
				new AccountSpecification().WithId(command.AccountId.Value).WithLock(),
				ct
			),
			{ Email: not null } when !string.IsNullOrWhiteSpace(command.Email) => await repository.Find(
				new AccountSpecification().WithEmail(command.Email).WithLock(),
				ct
			),
			{ Login: not null } when !string.IsNullOrWhiteSpace(command.Login) => await repository.Find(
				new AccountSpecification().WithLogin(command.Login).WithLock(),
				ct
			),
			_ => Error.Validation("Не предоставлен ни Email, ни логин, ни Id аккаунта для изменения Email."),
		};
}

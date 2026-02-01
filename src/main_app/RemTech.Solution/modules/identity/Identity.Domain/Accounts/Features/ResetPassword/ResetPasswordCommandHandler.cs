using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.ResetPassword;

/// <summary>
/// Обработчик команды для сброса пароля пользователя.
/// </summary>
/// <param name="accounts">Репозиторий аккаунтов.</param>
/// <param name="tickets">Репозиторий тикетов аккаунтов.</param>
[TransactionalHandler]
public sealed class ResetPasswordCommandHandler(IAccountsRepository accounts, IAccountTicketsRepository tickets)
	: ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
	/// <summary>
	/// Выполняет сброс пароля пользователя по команде.
	/// </summary>
	/// <param name="command">Команда сброса пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<ResetPasswordResult>> Execute(ResetPasswordCommand command, CancellationToken ct = default)
	{
		Result<Account> account = await ResolveAccount(command, ct);
		Result<Unit> canReset = CanResetPassword(account);
		Result<AccountTicket> ticket = CreateResetPasswordTicket(account, canReset);
		if (ticket.IsFailure)
		{
			return ticket.Error;
		}

		await tickets.Add(ticket.Value, ct);
		return ResetPasswordResult.From(account.Value, ticket.Value);
	}

	private static Result<AccountTicket> CreateResetPasswordTicket(Result<Account> account, Result<Unit> canReset)
	{
		if (account.IsFailure)
		{
			return account.Error;
		}

		return canReset.IsFailure
			? (Result<AccountTicket>)canReset.Error
			: account.Value.CreateTicket(AccountTicketPurposes.RESET_PASSWORD);
	}

	private static Result<Unit> CanResetPassword(Result<Account> account)
	{
		return account.IsFailure ? (Result<Unit>)account.Error : account.Value.CanResetPassword();
	}

	private async Task<Result<Account>> ResolveAccount(ResetPasswordCommand command, CancellationToken ct)
	{
		if (!string.IsNullOrWhiteSpace(command.Email))
		{
			return await FindAccountByEmail(command.Email, ct);
		}

		return !string.IsNullOrWhiteSpace(command.Login)
			? await FindAccountByLogin(command.Login, ct)
			: (Result<Account>)Error.Validation("Не предоставлен ни Email, ни логин для сброса пароля.");
	}

	private Task<Result<Account>> FindAccountByEmail(string email, CancellationToken ct)
	{
		AccountSpecification spec = new AccountSpecification().WithEmail(email).WithLock();
		return accounts.Find(spec, ct);
	}

	private Task<Result<Account>> FindAccountByLogin(string login, CancellationToken ct)
	{
		AccountSpecification spec = new AccountSpecification().WithLogin(login).WithLock();
		return accounts.Find(spec, ct);
	}
}

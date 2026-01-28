using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.ConfirmPasswordReset;

/// <summary>
/// Обработчик команды подтверждения сброса пароля пользователя.
/// </summary>
/// <param name="tokens">Репозиторий токенов обновления.</param>
/// <param name="accounts">Репозиторий аккаунтов.</param>
/// <param name="tickets">Репозиторий тикетов аккаунтов.</param>
/// <param name="hasher">Хешер паролей.</param>
/// <param name="requirements">Требования к паролю аккаунта.</param>
/// <param name="unitOfWork">Единица работы для аккаунтов.</param>
[TransactionalHandler]
public sealed class ConfirmResetPasswordHandler(
	IRefreshTokensRepository tokens,
	IAccountsRepository accounts,
	IAccountTicketsRepository tickets,
	IPasswordHasher hasher,
	IEnumerable<IAccountPasswordRequirement> requirements,
	IAccountsModuleUnitOfWork unitOfWork
) : ICommandHandler<ConfirmResetPasswordCommand, Unit>
{
	/// <summary>
	/// Выполняет подтверждение сброса пароля пользователя по команде.
	/// </summary>
	/// <param name="command">Команда подтверждения сброса пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<Unit>> Execute(ConfirmResetPasswordCommand command, CancellationToken ct = default)
	{
		Result<Account> account = await GetAccount(command, ct);
		if (account.IsFailure)
			return account.Error;

		Result<AccountTicket> ticket = await GetTicket(command, ct);
		if (ticket.IsFailure)
			return ticket.Error;

		Result<Unit> closing = account.Value.CloseTicket(ticket.Value);
		Result<Unit> changing = ChangePassword(command, account, closing);
		Result<Unit> saving = await SaveChanges(account, ticket, closing, changing, ct);
		return saving.IsFailure ? saving.Error : await Logout(command, ct);
	}

	/// <summary>
	/// Выполняет выход пользователя из всех сессий.
	/// </summary>
	/// <param name="command">Команда подтверждения сброса пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<Unit>> Logout(ConfirmResetPasswordCommand command, CancellationToken ct)
	{
		await tokens.Delete(command.AccountId, ct);
		return Unit.Value;
	}

	/// <summary>
	/// Сохраняет изменения аккаунта и тикета в хранилище.
	/// </summary>
	/// <param name="account">Аккаунт пользователя.</param>
	/// <param name="ticket">Тикет аккаунта.</param>
	/// <param name="closing">Результат закрытия тикета.</param>
	/// <param name="changing">Результат изменения пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения операции сохранения.</returns>
	public async Task<Result<Unit>> SaveChanges(
		Result<Account> account,
		Result<AccountTicket> ticket,
		Result<Unit> closing,
		Result<Unit> changing,
		CancellationToken ct
	)
	{
		if (account.IsFailure)
			return account.Error;
		if (ticket.IsFailure)
			return ticket.Error;
		if (closing.IsFailure)
			return closing.Error;
		if (changing.IsFailure)
			return changing.Error;
		await unitOfWork.Save(account.Value, ct);
		return Unit.Value;
	}

	/// <summary>
	/// Изменяет пароль пользователя.
	/// </summary>
	/// <param name="command">Команда подтверждения сброса пароля.</param>
	/// <param name="account">Аккаунт пользователя.</param>
	/// <param name="closing">Результат закрытия тикета.</param>
	/// <returns>Результат выполнения операции изменения пароля.</returns>
	public Result<Unit> ChangePassword(
		ConfirmResetPasswordCommand command,
		Result<Account> account,
		Result<Unit> closing
	)
	{
		if (account.IsFailure)
			return account.Error;
		if (closing.IsFailure)
			return closing.Error;
		AccountPassword password = AccountPassword.Create(command.NewPassword);
		return account.Value.ChangePassword(password, hasher, requirements);
	}

	/// <summary>
	/// Получает аккаунт пользователя по команде.
	/// </summary>
	/// <param name="command">Команда подтверждения сброса пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения операции получения аккаунта.</returns>
	public Task<Result<Account>> GetAccount(ConfirmResetPasswordCommand command, CancellationToken ct) =>
		accounts.Find(new AccountSpecification().WithId(command.AccountId).WithLock(), ct);

	/// <summary>
	/// Получает тикет сброса пароля по команде.
	/// </summary>
	/// <param name="command">Команда подтверждения сброса пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения операции получения тикета.</returns>
	public Task<Result<AccountTicket>> GetTicket(ConfirmResetPasswordCommand command, CancellationToken ct) =>
		tickets.Find(
			new AccountTicketSpecification()
				.WithTicketId(command.TicketId)
				.WithAccountId(command.AccountId)
				.WithLockRequired(),
			ct
		);
}

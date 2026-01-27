using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.ConfirmTicket;

/// <summary>
/// Обработчик команды подтверждения тикета пользователя.
/// </summary>
/// <param name="accounts">Репозиторий аккаунтов.</param>
/// <param name="accountTickets">Репозиторий тикетов аккаунтов.</param>
/// <param name="unitOfWork">Единица работы для аккаунтов.</param>
[TransactionalHandler]
public sealed class ConfirmTicketHandler(
	IAccountsRepository accounts,
	IAccountTicketsRepository accountTickets,
	IAccountsModuleUnitOfWork unitOfWork
) : ICommandHandler<ConfirmTicketCommand, Account>
{
	/// <summary>
	/// Выполняет подтверждение тикета пользователя по команде.
	/// </summary>
	/// <param name="command">Команда подтверждения тикета пользователя.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<Account>> Execute(ConfirmTicketCommand command, CancellationToken ct = default)
	{
		Result<Account> account = await GetRequiredAccount(command, ct);
		if (account.IsFailure)
			return account.Error;

		Result<AccountTicket> ticket = await GetRequiredAccountTicket(command, ct);
		if (ticket.IsFailure)
			return ticket.Error;

		Result<Unit> confirmation = ConfirmAccountTicket(ticket.Value, account.Value);
		if (confirmation.IsFailure)
			return confirmation.Error;

		await SaveChanges(account.Value, ticket.Value, ct);
		return account.Value;
	}

	private static Result<Unit> ConfirmAccountTicket(AccountTicket ticket, Account account)
	{
		Result<Unit> confirmation = ticket.FinishBy(account.Id.Value);
		if (confirmation.IsFailure)
			return confirmation.Error;

		if (ticket.Purpose == AccountTicketPurposes.EMAIL_CONFIRMATION_REQUIRED)
		{
			Result<Unit> activation = account.Activate();
			if (activation.IsFailure)
				return activation.Error;
		}

		return Unit.Value;
	}

	private async Task SaveChanges(Account account, AccountTicket ticket, CancellationToken ct)
	{
		await unitOfWork.Save(account, ct);
		await unitOfWork.Save(ticket, ct);
	}

	private Task<Result<Account>> GetRequiredAccount(ConfirmTicketCommand command, CancellationToken ct)
	{
		AccountSpecification specification = new AccountSpecification().WithId(command.AccountId).WithLock();
		return accounts.Find(specification, ct);
	}

	private Task<Result<AccountTicket>> GetRequiredAccountTicket(ConfirmTicketCommand command, CancellationToken ct)
	{
		AccountTicketSpecification specification = new AccountTicketSpecification()
			.WithAccountId(command.AccountId)
			.WithTicketId(command.TicketId)
			.NotFinished()
			.WithLockRequired();
		return accountTickets.Find(specification, ct);
	}
}

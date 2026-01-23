using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.ConfirmTicket;

[TransactionalHandler]
public sealed class ConfirmTicketHandler(
	IAccountsRepository accounts,
	IAccountTicketsRepository accountTickets,
	IAccountsModuleUnitOfWork unitOfWork
) : ICommandHandler<ConfirmTicketCommand, Account>
{
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

	private async Task SaveChanges(Account account, AccountTicket ticket, CancellationToken ct)
	{
		await unitOfWork.Save(account, ct);
		await unitOfWork.Save(ticket, ct);
	}

	private static Result<Unit> ConfirmAccountTicket(AccountTicket ticket, Account account)
	{
		Result<Unit> confirmation = ticket.FinishBy(account.Id.Value);
		if (confirmation.IsFailure)
			return confirmation.Error;

		if (ticket.Purpose == AccountTicketPurposes.EmailConfirmationRequired)
		{
			Result<Unit> activation = account.Activate();
			if (activation.IsFailure)
				return activation.Error;
		}

		return Unit.Value;
	}

	private async Task<Result<Account>> GetRequiredAccount(ConfirmTicketCommand command, CancellationToken ct)
	{
		AccountSpecification specification = new AccountSpecification().WithId(command.AccountId).WithLock();
		return await accounts.Find(specification, ct);
	}

	private async Task<Result<AccountTicket>> GetRequiredAccountTicket(
		ConfirmTicketCommand command,
		CancellationToken ct
	)
	{
		AccountTicketSpecification specification = new AccountTicketSpecification()
			.WithAccountId(command.AccountId)
			.WithTicketId(command.TicketId)
			.NotFinished()
			.WithLockRequired();
		return await accountTickets.Find(specification, ct);
	}
}

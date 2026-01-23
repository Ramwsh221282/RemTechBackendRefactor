using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.ConfirmPasswordReset;

public sealed record ConfirmResetPasswordCommand(Guid AccountId, Guid TicketId, string NewPassword) : ICommand;

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

	public async Task<Result<Unit>> Logout(ConfirmResetPasswordCommand command, CancellationToken ct)
	{
		await tokens.Delete(command.AccountId, ct);
		return Unit.Value;
	}

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

	public async Task<Result<Account>> GetAccount(ConfirmResetPasswordCommand command, CancellationToken ct) =>
		await accounts.Find(new AccountSpecification().WithId(command.AccountId).WithLock(), ct);

	public async Task<Result<AccountTicket>> GetTicket(ConfirmResetPasswordCommand command, CancellationToken ct) =>
		await tickets.Find(
			new AccountTicketSpecification()
				.WithTicketId(command.TicketId)
				.WithAccountId(command.AccountId)
				.WithLockRequired(),
			ct
		);
}

using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ResetPassword;

public sealed record ResetPasswordCommand(string? Login, string? Email) : ICommand;

public sealed record ResetPasswordResult(
    Guid AccountId,
    string AccountEmail,
    Guid TicketId,
    string TicketPurpose
)
{
    public static ResetPasswordResult From(Account account, AccountTicket ticket) =>
        new(account.Id.Value, account.Email.Value, ticket.TicketId, ticket.Purpose);
}

public sealed class ResetPasswordCommandHandler(
    IAccountsRepository accounts,
    IAccountTicketsRepository tickets
) : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
    public async Task<Result<ResetPasswordResult>> Execute(
        ResetPasswordCommand command,
        CancellationToken ct = default
    )
    {
        Result<Account> account = await ResolveAccount(command, ct);
        Result<Unit> canReset = CanResetPassword(account);
        Result<AccountTicket> ticket = CreateResetPasswordTicket(account, canReset);
        if (ticket.IsFailure)
            return ticket.Error;
        await tickets.Add(ticket.Value, ct);
        return ResetPasswordResult.From(account.Value, ticket.Value);
    }

    private async Task<Result<Account>> ResolveAccount(
        ResetPasswordCommand command,
        CancellationToken ct
    )
    {
        if (!string.IsNullOrWhiteSpace(command.Email))
            return await FindAccountByEmail(command.Email, ct);
        if (!string.IsNullOrWhiteSpace(command.Login))
            return await FindAccountByLogin(command.Login, ct);
        return Error.Validation("Не предоставлен ни Email, ни логин для сброса пароля.");
    }

    private static Result<AccountTicket> CreateResetPasswordTicket(
        Result<Account> account,
        Result<Unit> canReset
    )
    {
        if (account.IsFailure)
            return account.Error;
        if (canReset.IsFailure)
            return canReset.Error;
        return account.Value.CreateTicket(AccountTicketPurposes.ResetPassword);
    }

    private async Task<Result<Account>> FindAccountByEmail(string email, CancellationToken ct)
    {
        AccountSpecification spec = new AccountSpecification().WithEmail(email).WithLock();
        return await accounts.Get(spec, ct);
    }

    private async Task<Result<Account>> FindAccountByLogin(string login, CancellationToken ct)
    {
        AccountSpecification spec = new AccountSpecification().WithLogin(login).WithLock();
        return await accounts.Get(spec, ct);
    }

    private static Result<Unit> CanResetPassword(Result<Account> account)
    {
        if (account.IsFailure)
            return account.Error;
        return account.Value.CanResetPassword();
    }
}

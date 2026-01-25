using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Tickets;

public sealed class AccountTicket(AccountId accountId, Guid ticketId, bool finished, string purpose)
{
    private AccountTicket(AccountTicket ticket)
        : this(ticket.AccountId, ticket.TicketId, ticket.Finished, ticket.Purpose) { }

    public AccountId AccountId { get; } = accountId;
    public Guid TicketId { get; } = ticketId;
    public bool Finished { get; private set; } = finished;
    public string Purpose { get; private set; } = purpose;

    public Result<Unit> Finish()
    {
        if (Finished)
            return Error.Conflict("Заявка уже выполнена.");
        Finished = true;
        return Result.Success(Unit.Value);
    }

    public Result<Unit> FinishBy(Guid finisherId)
    {
        if (Finished)
            return Error.Conflict("Заявка уже выполнена.");
        if (finisherId != AccountId.Value)
            return Error.Forbidden("Заявка не привязана к учетной записи.");
        Finished = true;
        return Result.Success(Unit.Value);
    }

    public AccountTicket Clone() => new(this);

    public static Result<AccountTicket> New(Guid accountId, string purpose)
    {
        return string.IsNullOrWhiteSpace(purpose)
            ? (Result<AccountTicket>)Error.Validation("Причина создания заявки не указана.")
            : (Result<AccountTicket>)new AccountTicket(AccountId.Create(accountId), Guid.NewGuid(), false, purpose);
    }

    public static Result<AccountTicket> New(AccountId accountId, string purpose) => New(accountId.Value, purpose);

    public static Result<AccountTicket> New(Account account, string purpose) => New(account.Id, purpose);
}

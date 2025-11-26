using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.AccountTickets.Decorators;

public sealed class PersistingTicket(IAccountTicket ticket) : AccountTicketEnvelope(ticket)
{
    private readonly IAccountTicket _ticket = ticket;

    public override async Task<Result<Unit>> Register(
        IAccountTicketsStorage storage, 
        CancellationToken ct)
    {
        Result<Unit> result = await _ticket.Register(storage, ct);
        if (result.IsFailure) return result.Error;
        Result<Unit> validation = await ValidateTicketHasNoDuplicateAccountTypeRelation(storage, ct);
        if (validation.IsFailure) return validation.Error;
        await storage.Persist(_ticket, ct);
        return result;
    }

    private async Task<Result<Unit>> ValidateTicketHasNoDuplicateAccountTypeRelation(
        IAccountTicketsStorage storage,
        CancellationToken ct
        )
    {
        AccountTicketData data = _ticket.Representation();
        AccountTicketQueryArgs args = new(AccountId: data.AccountId, Type: data.Type);
        IAccountTicket? ticket = await storage.Fetch(args, ct);
        if (ticket != null) return Error.Conflict($"Заявка: {args.Type} уже имеется для учетной записи.");
        return Unit.Value;
    }
}
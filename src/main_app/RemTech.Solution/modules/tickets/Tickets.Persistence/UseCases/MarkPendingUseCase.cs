using RemTech.Functional.Extensions;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Persistence.UseCases;

public static class MarkPendingUseCase
{
    public static MarkPending MarkPending(MarkPending origin, TicketsStorage storage) => async (args) =>
    {
        QueryTicketArgs query = new(TicketId: args.TicketId, WithLock: true);
        Optional<Ticket> ticket = await storage.Find(query, args.Ct);
        Result<Ticket> pending = await origin(args with { Target = ticket });
        if (pending.IsFailure) return pending.Error;
        Result<Unit> saving = await storage.Update(pending, args.Ct);
        if (saving.IsFailure) return saving.Error;
        return pending;
    };
}
using RemTech.Functional.Extensions;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Persistence.UseCases;

public static class RegisterTicketUseCase
{
    public static RegisterTicket RegisterTicket(RegisterTicket origin, TicketsStorage storage) => async args =>
    {
        Result<Ticket> ticket = await origin(args);
        if (ticket.IsFailure) return ticket.Error;
        Result<Unit> saving = await storage.Insert(ticket, args.Ct);
        if (saving.IsFailure) return saving.Error;
        return ticket;
    };
}
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Persistence.UseCases;

public static class ActivateTicketUseCase
{
    public static ActivateTicket ActivateTicket(ActivateTicket origin, TicketsStorage storage) => async args =>
    {
        QueryTicketArgs query = new(TicketId: args.CreatorId, WithLock: true);
        Optional<Ticket> ticket = await storage.Find(query, args.Ct);
        Result<Ticket> activated = await origin(args with { Ticket = ticket });
        if (activated.IsFailure) return activated.Error;
        Result<Unit> updating = await storage.Update(activated, args.Ct);
        if (updating.IsFailure) return updating.Error;
        return activated;
    };

    extension(ActivateTicket origin)
    {
        public ActivateTicket WithPersistence(IServiceProvider sp)
        {
            return ActivateTicket(origin, sp.Resolve<TicketsStorage>());
        }
    }
}
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Persistence.UseCases;

public static class RegisterTicketUseCase
{
    private static RegisterTicket RegisterTicket(RegisterTicket origin, TicketsStorage storage) => async args =>
    {
        Result<Ticket> ticket = await origin(args);
        if (ticket.IsFailure) return ticket.Error;
        Result<Unit> saving = await storage.Insert(ticket, args.Ct);
        if (saving.IsFailure) return saving.Error;
        return ticket;
    };

    private static RegisterTicket RegisterTicket(RegisterTicket origin, NpgSqlSession session) => async args =>
    {
        CancellationToken ct = args.Ct;
        await session.GetTransaction(ct);
        Result<Ticket> ticket = await origin(args);
        if (ticket.IsFailure) return ticket.Error;
        if (!await session.Commited(ct)) return Error.Application("Не удается зафиксировать изменения.");
        return ticket;
    };
    
    extension(RegisterTicket origin)
    {
        public RegisterTicket WithPersistence(IServiceProvider sp)
        {
            return RegisterTicket(origin, sp.Resolve<TicketsStorage>());
        }

        public RegisterTicket WithTransaction(IServiceProvider sp)
        {
            NpgSqlSession session = sp.Resolve<NpgSqlSession>();
            return RegisterTicket(origin, session);
        }
    }
}
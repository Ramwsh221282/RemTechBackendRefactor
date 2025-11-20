using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;
using Tickets.Core;
using Tickets.Core.Contracts;
using Tickets.Core.Snapshots;

namespace Tickets.Logging;

public static class RegisterTicketUseCase
{
    public static RegisterTicket RegisterTicket(RegisterTicket origin, ILogger logger) => async args =>
    {
        Result<Ticket> ticket = await origin(args);
        if (ticket.IsSuccess)
        {
            TicketSnapshot snap = ticket.Value.Snapshot();
            object[] parameters = [args.CreatorId, args.Type, ticket.IsSuccess, snap.Metadata.Extra ?? string.Empty];
            logger.Information(
                """
                Создание заявки:
                Creator Id: {Id}
                Ticket type: {Type}
                IsSuccess: {IsSuccess}
                Extra: {Extra}
                """, parameters);
        }
        if (ticket.IsFailure)
        {
            object[] parameters = [args.CreatorId, args.Type, ticket.IsSuccess, ticket.Error.Message];
            logger.Error(
                """
                Создание заявки:
                Creator Id: {Id}
                Ticket type: {Type}
                IsSuccess: {IsSuccess}
                Error: {Error}
                """, parameters);
        }
        return ticket;
    };

    extension(RegisterTicket origin)
    {
        public RegisterTicket WithLogging(IServiceProvider sp)
        {
            return RegisterTicket(origin, sp.Resolve<ILogger>());
        }
    }
}
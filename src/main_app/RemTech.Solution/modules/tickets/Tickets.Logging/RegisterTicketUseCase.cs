using RemTech.Functional.Extensions;
using Serilog;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.Logging;

public static class RegisterTicketUseCase
{
    public static RegisterTicket RegisterTicket(RegisterTicket origin, ILogger logger) => async args =>
    {
        Result<Ticket> ticket = await origin(args);
        object[] parameters = [args.CreatorId, args.Type, ticket.IsSuccess, ticket.Error.Message];
        if (ticket.IsSuccess)
        {
            logger.Information(
                """
                Создание заявки:
                Creator Id: {Id}
                Ticket type: {Type}
                IsSuccess: {IsSuccess}
                """, parameters);
        }
        if (ticket.IsFailure)
        {
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
}
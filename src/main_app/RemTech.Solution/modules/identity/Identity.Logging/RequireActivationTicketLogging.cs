using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Tickets;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class RequireActivationTicketLogging
{
    internal static RequireActivationTicket RequireActivationTicket(
        ILogger logger,
        RequireActivationTicket origin) => async args =>
    {
        Result<SubjectTicket> ticket = await origin(args);
        
        if (ticket.IsFailure)
        {
            logger.Error(
                """
                Заявка на активацию учетной записи
                Subject ID: {SId}
                Ticket ID: {TId}
                Создана: {Success}
                Ошибка: {Error}
                """, [args.SubjectId, ticket.IsSuccess, ticket.Error.Message]);
        }
        else
        {
            Guid id = ticket.Value.Snapshot().Id;
            logger.Information(
                """
                Заявка на активацию учетной записи
                Subject ID: {SId}
                Ticket ID: {TId}
                Создана: {Success}
                """, [args.SubjectId, id, ticket.IsSuccess, ticket.Error.Message]);
        }

        return ticket;
    };

    extension(RequireActivationTicket origin)
    {
        public RequireActivationTicket WithLogging(IServiceProvider sp)
        {
            ILogger logger = sp.Resolve<ILogger>();
            return RequireActivationTicket(logger, origin);
        }
    }
}
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
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
        Result<RequireActivationTicketResult> ticket = await origin(args);
        
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
            SubjectSnapshot subjectSnap = ticket.Value.Subject.Snapshot();
            SubjectTicketSnapshot ticketSnap = ticket.Value.Ticket.Snapshot();
            
            Guid id = subjectSnap.Id;
            Guid ticketId = ticketSnap.Id;
            string email = subjectSnap.Email;
            
            logger.Information(
                """
                Заявка на активацию учетной записи
                Subject ID: {SId}
                Ticket ID: {TId}
                Subject Email: {email}
                Создана: {Success}
                """, [id, ticketId, email, ticket.IsSuccess, ticket.Error.Message]);
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
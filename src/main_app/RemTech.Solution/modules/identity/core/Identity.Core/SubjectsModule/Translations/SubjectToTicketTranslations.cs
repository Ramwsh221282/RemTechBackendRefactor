using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.TicketsModule;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Translations;

public static class SubjectToTicketTranslations
{
    public static Result<SubjectTicket> TicketBecomesSubjectTicket(Ticket ticket)
    {
        return new SubjectTicket(ticket.Id, ticket.Type, ticket.Active).Validated();
    }

    public static Result<Ticket> SubjectTicketBecomesTicket(SubjectTicket ticket)
    {
        Result<Guid> creatorId = ticket.CreatorId();
        if (creatorId.IsFailure) return creatorId.Error;
        return new Ticket(ticket._id, creatorId, ticket._type, DateTime.UtcNow, true);
    }
}
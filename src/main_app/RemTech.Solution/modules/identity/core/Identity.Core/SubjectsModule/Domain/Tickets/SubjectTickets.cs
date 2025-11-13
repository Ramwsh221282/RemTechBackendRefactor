using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Domain.Tickets;

public sealed record SubjectTickets(SubjectTicket[] Tickets)
{
    public bool ContainsLastNotActiveWithSameType(SubjectTicket ticket)
    {
        Optional<SubjectTicket> optional = Tickets.TryFind(ticket.OfType());
        return optional.HasValue;
    }

    public SubjectTickets WithoutLastActiveOfType(SubjectTicket ticket)
    {
        return new SubjectTickets(Tickets.Without(ticket, ticket.OfType()));
    }

    public SubjectTickets With(SubjectTicket ticket)
    {
        return new SubjectTickets(Tickets.With(ticket));
    }

    public static SubjectTickets Empty() => new([]);
    internal SubjectTickets Copy() => new([..Tickets]);
}
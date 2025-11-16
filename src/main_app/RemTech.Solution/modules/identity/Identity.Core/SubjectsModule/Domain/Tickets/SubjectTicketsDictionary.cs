namespace Identity.Core.SubjectsModule.Domain.Tickets;

public sealed class SubjectTicketsDictionary
{
    private readonly Dictionary<string, SubjectTicket> _tickets;

    public bool Contains(SubjectTicket ticket)
    {
        return ticket.BelongsTo(_tickets);
    }

    public SubjectTicketsDictionary Add(SubjectTicket ticket)
    {
        ticket.AppendTo(_tickets);
        return new  SubjectTicketsDictionary(_tickets);
    }

    public SubjectTicketsDictionary Remove(SubjectTicket ticket)
    {
        ticket.LeaveFrom(_tickets);
        return new  SubjectTicketsDictionary(_tickets);
    }

    public SubjectTicketSnapshot[] Snapshot()
    {
        return _tickets.Values.Select(v => v.Snapshot()).ToArray();
    }
    
    public SubjectTicketsDictionary()
    {
        _tickets = [];
    }

    public SubjectTicketsDictionary(Dictionary<string, SubjectTicket> tickets)
    {
        _tickets = tickets;
    }
    
    public SubjectTicketsDictionary(IEnumerable<SubjectTicket> tickets)
    {
        _tickets = tickets.ToDictionary(k => k._type);
    }
}
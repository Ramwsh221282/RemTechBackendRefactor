namespace Identity.Core.SubjectsModule.Domain.Tickets;

public static class SubjectTicketFactoryModule
{
    extension(SubjectTicket)
    {
        public static Result<SubjectTicketsDictionary> Create(IEnumerable<SubjectTicketSnapshot> snapshots)
        {
            SubjectTicketsDictionary dict = new();
            foreach (SubjectTicketSnapshot ticket in snapshots)
            {
                Result<SubjectTicket> result = Create(ticket);
                if (result.IsFailure) return result.Error;
                if (dict.Contains(result)) return Validation("Заявки учетной записи не могут повторяться по типам.");
                dict = dict.Add(result);
            }

            return dict;
        }
        
        public static Result<SubjectTicket> Create(Guid id, string type)
        {
            SubjectTicket ticket = new(id, type, false);
            return ticket.Validated();
        }

        public static Result<SubjectTicket> Create(SubjectTicketSnapshot snapshot)
        {
            return snapshot.CreatorId.NoValue 
                ? Validation("Заявка не имеет идентификатор создателя.") 
                : Create(snapshot.CreatorId.Value, snapshot.Id, snapshot.Type, snapshot.Active);
        }
        
        public static SubjectTicket Create(string type)
        {
            SubjectTicket ticket = new(Guid.NewGuid(), type, false);
            return ticket;
        }
        
        public static Result<SubjectTicket> Create(Guid id, string type, bool active)
        {
            SubjectTicket ticket = new(id, type, active);
            return ticket.Validated();
        }

        public static Result<SubjectTicket> Create(Guid subjectId, Guid id, string type, bool active)
        {
            SubjectTicket ticket = new(subjectId, id, type, active);
            return ticket.Validated();
        }
    }
}
namespace Identity.Core.TicketsModule;

public static class TicketsFactoryModule
{
    extension(Ticket)
    {
        public static Ticket New(Guid creatorId, string type)
        {
            return new Ticket(Guid.NewGuid(), creatorId, type, DateTime.UtcNow, true);
        }

        public static Ticket Create(Guid id, Guid creatorId, string type)
        {
            DateTime created = DateTime.UtcNow;
            Optional<DateTime> closed = None<DateTime>();
            bool active = true;
            return new Ticket(id, creatorId, type, created, closed, active).Validated();
        }
        
        public static Ticket Create(TicketSnapshot snapshot)
        {
            return new Ticket(
                snapshot.Id, 
                snapshot.CreatorId, 
                snapshot.Type, 
                snapshot.Created, 
                FromNullable(snapshot.Closed), 
                snapshot.Active);
        }
    }
}
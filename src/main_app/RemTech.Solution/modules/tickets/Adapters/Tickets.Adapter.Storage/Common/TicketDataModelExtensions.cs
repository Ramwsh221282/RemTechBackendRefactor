using Tickets.Adapter.Storage.DataModels;
using Tickets.Domain.Tickets;

namespace Tickets.Adapter.Storage.Common;

internal static class TicketDataModelExtensions
{
    public static TicketDataModel ToDataModel(this Ticket ticket)
    {
        Guid id = ticket.Id.Value;
        DateTime created = ticket.LifeTime.Created;
        DateTime? deleted = ticket.LifeTime.Deleted;
        string content = ticket.Content.Value;

        return new TicketDataModel()
        {
            Id = id,
            Created = created,
            Deleted = deleted,
            Content = content,
        };
    }
}

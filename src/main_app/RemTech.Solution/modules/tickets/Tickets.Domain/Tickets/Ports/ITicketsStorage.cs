using RemTech.Core.Shared.Result;

namespace Tickets.Domain.Tickets.Ports;

public interface ITicketsStorage
{
    Task<Status> Add(Ticket ticket, CancellationToken ct = default);
}

using RemTech.Core.Shared.Result;
using Tickets.Adapter.Storage.Common;
using Tickets.Adapter.Storage.DataModels;
using Tickets.Domain.Tickets;
using Tickets.Domain.Tickets.Ports;

namespace Tickets.Adapter.Storage.Implementations;

public sealed class TicketsStorage(Serilog.ILogger logger, TicketsDbContext context)
    : ITicketsStorage
{
    public async Task<Status> Add(Ticket ticket, CancellationToken ct = default)
    {
        TicketDataModel dm = ticket.ToDataModel();
        return await Add(dm, ct);
    }

    internal async Task<Status> Add(TicketDataModel ticket, CancellationToken ct = default)
    {
        await context.Tickets.AddAsync(ticket, ct);
        await context.SaveChangesAsync(ct);
        return Status.Success();
    }
}

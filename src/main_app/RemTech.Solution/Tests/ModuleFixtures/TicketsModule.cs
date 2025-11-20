using RemTech.BuildingBlocks.DependencyInjection;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tests.ModuleFixtures;

public sealed class TicketsModule
{
    private readonly IServiceProvider _sp;

    public TicketsModule(IServiceProvider sp)
    {
        _sp = sp;
    }

    public async Task<bool> HasTickets()
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        TicketsStorage storage = scope.ServiceProvider.Resolve<TicketsStorage>();
        return await storage.HasAny(CancellationToken.None);
    }

    public async Task<IEnumerable<Ticket>> GetTickets()
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        TicketsStorage storage = scope.ServiceProvider.Resolve<TicketsStorage>();
        return await storage.FindMany(new QueryTicketArgs(), CancellationToken.None);
    }
}
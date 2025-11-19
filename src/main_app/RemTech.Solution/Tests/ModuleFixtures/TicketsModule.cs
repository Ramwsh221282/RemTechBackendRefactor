using RemTech.BuildingBlocks.DependencyInjection;
using Tickets.Core.Contracts;

namespace Tests.ModuleFixtures;

public sealed class TicketsModule
{
    private readonly IServiceProvider _sp;

    public TicketsModule(IServiceProvider sp)
    {
        _sp = sp;
    }

    public async Task<bool> HasTickets(int waitSeconds = 0)
    {
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(waitSeconds));
        while (!cts.IsCancellationRequested)
        {
            try
            {
                await using AsyncServiceScope scope = _sp.CreateAsyncScope();
                TicketsStorage storage = scope.ServiceProvider.Resolve<TicketsStorage>();
                return await storage.HasAny(CancellationToken.None);
            }
            catch(OperationCanceledException)
            {
                return false;
            }
        }

        return false;
    }
}
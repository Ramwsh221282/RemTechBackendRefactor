using RemTech.BuildingBlocks.DependencyInjection;
using Tickets.Core.Contracts;

namespace Tests.Tickets;

public sealed class TicketsModule
{
    private readonly Lazy<IServiceProvider> _sp;
    private IServiceProvider Sp => _sp.Value;

    public TicketsModule(Lazy<IServiceProvider> sp)
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
                await using AsyncServiceScope scope = Sp.CreateAsyncScope();
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
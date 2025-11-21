using CompositionRoot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Tickets.EventListeners;

namespace CompositionRoot.tickets.DependencyInjection.TicketRouters;

[DependencyInjectionClass]
internal static class DependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTicketRouters();
    }
}
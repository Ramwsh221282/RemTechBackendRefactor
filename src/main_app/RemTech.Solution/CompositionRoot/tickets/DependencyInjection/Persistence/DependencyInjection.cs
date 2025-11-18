using CompositionRoot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Persistence;

namespace CompositionRoot.tickets.DependencyInjection.Persistence;

[DependencyInjectionClass]
internal static class DependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTicketsPersistence();
    }
}
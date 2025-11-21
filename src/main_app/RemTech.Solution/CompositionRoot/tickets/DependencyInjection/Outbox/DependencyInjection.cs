using CompositionRoot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Outbox;

namespace CompositionRoot.tickets.DependencyInjection.Outbox;

[DependencyInjectionClass]
internal static class DependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTicketOutbox();
    }
}
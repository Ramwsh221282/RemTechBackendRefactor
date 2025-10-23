using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.DomainEvents;
using Tickets.Adapter.Storage.Implementations;

namespace Tickets.Adapter.Storage;

public static class TicketsStorageAdapterDependencyInjection
{
    public static void RegisterTicketsStorageAdapter(this IServiceCollection services)
    {
        services.AddScoped<TicketsDbContext>();
        services.AddDomainEventHandlers(typeof(TicketsStorageAdapterDependencyInjection).Assembly);
    }
}

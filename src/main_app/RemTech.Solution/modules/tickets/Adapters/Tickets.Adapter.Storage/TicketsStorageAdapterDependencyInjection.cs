using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.DomainEvents;
using Tickets.Adapter.Storage.Implementations;
using Tickets.Domain.Tickets.Ports;

namespace Tickets.Adapter.Storage;

public static class TicketsStorageAdapterDependencyInjection
{
    public static void RegisterTicketsStorageAdapter(this IServiceCollection services)
    {
        services.AddScoped<ITicketsStorage, TicketsStorage>();
        services.AddScoped<TicketsDbContext>();
        services.AddDomainEventHandlers(typeof(TicketsStorageAdapterDependencyInjection).Assembly);
    }
}

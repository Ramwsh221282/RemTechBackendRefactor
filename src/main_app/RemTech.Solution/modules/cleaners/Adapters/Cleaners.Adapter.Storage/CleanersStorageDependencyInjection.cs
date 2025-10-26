using Cleaners.Adapter.Storage.Storages;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.DomainEvents;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Adapter.Storage;

public static class CleanersStorageDependencyInjection
{
    public static void AddCleanersStorageAdapter(this IServiceCollection services)
    {
        var assembly = typeof(CleanersStorageDependencyInjection).Assembly;
        services.AddDomainEventHandlers(assembly);
        services.AddStorageModelMappings(assembly);
        services.AddScoped<ICleanersStorage, CleanersStorage>();
        services.AddScoped<CleanersDbContext>();
    }
}

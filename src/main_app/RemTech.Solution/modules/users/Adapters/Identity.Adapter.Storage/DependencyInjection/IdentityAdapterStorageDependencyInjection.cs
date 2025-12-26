using Identity.Adapter.Storage.Seeding;
using Identity.Adapter.Storage.Storages;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Users.Ports.Storage;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.DomainEvents;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Identity.Adapter.Storage.DependencyInjection;

public static class IdentityAdapterStorageDependencyInjection
{
    public static void InjectIdentityStorageAdapter(this IServiceCollection services)
    {
        var assembly = typeof(IdentityDbContext).Assembly;
        services.AddScoped<IUsersStorage, UsersStorage>();
        services.AddScoped<IRolesStorage, RolesStorage>();
        services.AddScoped<IdentityDbContext>();
        services.AddDomainEventHandlers(assembly);
        services.AddStorageModelMappings(assembly);
        services.AddScoped<IUserSeeding, UserSeeding>();
        services.AddScoped<IRolesSeeding, RolesSeeding>();
    }
}

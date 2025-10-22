using Identity.Adapter.Storage.Storages;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Users.Ports.Storage;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Adapter.Storage.DependencyInjection;

public static class IdentityAdapterStorageDependencyInjection
{
    public static void InjectIdentityStorageAdapter(this IServiceCollection services)
    {
        services.AddScoped<IUsersStorage, UsersStorage>();
        services.AddScoped<IRolesStorage, RolesStorage>();
        services.AddScoped<IdentityDbContext>();
        services.AddDomainEventHandlers(typeof(IdentityDbContext).Assembly);
    }
}

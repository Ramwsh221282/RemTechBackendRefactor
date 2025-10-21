using System.Reflection;
using Identity.Adapter.Storage.EventHandlers;
using Identity.Adapter.Storage.Storages;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Users.Ports.EventHandlers;
using Identity.Domain.Users.Ports.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Adapter.Storage.DependencyInjection;

public static class IdentityAdapterStorageDependencyInjection
{
    public static void InjectIdentityStorageAdapter(this IServiceCollection services)
    {
        services.AddScoped<IUsersStorage, UsersStorage>();
        services.AddScoped<IRolesStorage, RolesStorage>();
        services.AddScoped<IdentityDbContext>();
        services.AddScoped<IIdentityUserEventHandler, StorageAdapterEventHandler>();
        services.InjectEventHandlers();
    }

    private static void InjectEventHandlers(this IServiceCollection services)
    {
        Assembly assembly = typeof(IIdentityStorageAdapterEventHandler<>).Assembly;
        Type genericInterface = typeof(IIdentityStorageAdapterEventHandler<>);

        Type[] classes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass)
            .Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
            )
            .ToArray();

        foreach (Type @class in classes)
        {
            Type[] interfaces = @class
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
                .ToArray();

            foreach (Type interfaceType in interfaces)
                services.AddScoped(interfaceType, @class);
        }
    }
}

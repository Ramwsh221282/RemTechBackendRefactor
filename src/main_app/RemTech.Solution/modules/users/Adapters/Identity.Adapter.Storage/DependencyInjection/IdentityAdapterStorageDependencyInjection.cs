using Dapper.FluentMap;
using Dapper.FluentMap.Configuration;
using Dapper.FluentMap.Mapping;
using Identity.Adapter.Storage.Seeding;
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
        services.AddScoped<IUserSeeding, UserSeeding>();
        services.AddScoped<IRolesSeeding, RolesSeeding>();
        AddStorageModelMappings();
    }

    private static void AddStorageModelMappings()
    {
        Type baseMapType = typeof(EntityMap<>);

        IEnumerable<Type> mapperConfigurations = typeof(IdentityDbContext)
            .Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t =>
                t.BaseType is { IsGenericType: true }
                && t.BaseType.GetGenericTypeDefinition() == baseMapType
            );

        FluentMapper.Initialize(config =>
        {
            foreach (Type mapperConfig in mapperConfigurations)
            {
                Type entityType = mapperConfig.BaseType!.GetGenericArguments()[0];
                object instance = Activator.CreateInstance(mapperConfig)!;

                typeof(FluentMapConfiguration)
                    .GetMethod("AddMap")!
                    .MakeGenericMethod(entityType)
                    .Invoke(config, [instance]);
            }
        });
    }
}

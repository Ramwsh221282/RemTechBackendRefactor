using System.Reflection;
using Dapper.FluentMap;
using Dapper.FluentMap.Configuration;
using Dapper.FluentMap.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Module.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static T GetService<T>(this IServiceProvider provider)
        where T : class
    {
        return provider.GetRequiredService<T>();
    }

    public static T GetService<T>(this IServiceScope scope)
        where T : class
    {
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    public static async Task<T> ScopedExecution<T, TService>(
        this IServiceProvider provider,
        Func<AsyncServiceScope, TService> scopeService,
        Func<TService, Task<T>> resultFn
    )
        where TService : class
    {
        await using var scope = provider.CreateAsyncScope();
        var service = scopeService(scope);
        return await resultFn(service);
    }

    public static void AddStorageModelMappings(this IServiceCollection services, Assembly assembly)
    {
        Type baseMapType = typeof(EntityMap<>);

        IEnumerable<Type> mapperConfigurations = assembly
            .GetTypes()
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

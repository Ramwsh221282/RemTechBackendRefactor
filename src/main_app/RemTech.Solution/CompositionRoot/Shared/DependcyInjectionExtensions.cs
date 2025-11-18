using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace CompositionRoot.Shared;

public static class DependcyInjectionExtensions
{
    public static void RegisterModules(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependcyInjectionExtensions).Assembly;
        
        Type[] array = assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<DependencyInjectionClassAttribute>() is not null)
            .ToArray();

        foreach (Type type in array)
            type.GetMethods()
                .First(m => m.GetCustomAttribute<DependencyInjectionMethodAttribute>() is not null)
                .Invoke(null, [services]);
    }

    public static void ApplyModuleMigrations(this IServiceProvider sp)
    {
        var mainUpgrader = sp.GetRequiredService<PgVectorUpgrader>();
        mainUpgrader.ApplyMigrations();
        
        IEnumerable<IDbUpgrader> upgraders = sp.GetServices<IDbUpgrader>();
        foreach (IDbUpgrader upgrader in upgraders)
            upgrader.ApplyMigrations();
    }
}
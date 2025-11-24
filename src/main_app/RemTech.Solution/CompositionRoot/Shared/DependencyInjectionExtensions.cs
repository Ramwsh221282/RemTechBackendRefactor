using System.Reflection;
using Mailing.CompositionRoot;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace CompositionRoot.Shared;

public static class DependencyInjectionExtensions
{
    public static void RegisterModules(this IServiceCollection services)
    {
        services.AddMailingModule();
        Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;
        
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
        PgVectorUpgrader mainUpgrader = sp.GetRequiredService<PgVectorUpgrader>();
        mainUpgrader.ApplyMigrations();
        
        IEnumerable<IDbUpgrader> upgraders = sp.GetServices<IDbUpgrader>();
        foreach (IDbUpgrader upgrader in upgraders)
            upgrader.ApplyMigrations();
    }
}
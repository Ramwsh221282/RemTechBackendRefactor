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
}

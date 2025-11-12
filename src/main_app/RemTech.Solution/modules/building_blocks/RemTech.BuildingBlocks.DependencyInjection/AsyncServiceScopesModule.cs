using Microsoft.Extensions.DependencyInjection;

namespace RemTech.BuildingBlocks.DependencyInjection;

public static class AsyncServiceScopesModule
{
    extension(AsyncServiceScope scope)
    {
        public TService Resolve<TService>() where TService : notnull
        {
            return scope.ServiceProvider.GetRequiredService<TService>();
        }
    }
}
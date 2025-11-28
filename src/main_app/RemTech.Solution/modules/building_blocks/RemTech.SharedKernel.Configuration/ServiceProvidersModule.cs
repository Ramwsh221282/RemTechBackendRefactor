using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configuration;

public static class ServiceProvidersModule
{
    extension(IServiceProvider provider)
    {
        public async Task<TResult> ExecuteInScopeAsync<TResult>(Func<AsyncServiceScope, Task<TResult>> func)
        {
            await using AsyncServiceScope scope = provider.CreateAsyncScope();
            TResult result = await func(scope);
            return result;
        }

        public TService Resolve<TService>() where TService : notnull
        {
            return provider.GetRequiredService<TService>();
        }

        public async Task<TResult> ExecuteInScope<TRequiredService, TResult>(
            Func<TRequiredService, Task<TResult>> func)
            where TRequiredService : notnull
        {
            await using AsyncServiceScope scope = provider.CreateAsyncScope();
            TRequiredService service = scope.Resolve<TRequiredService>();
            return await func(service);
        }
    }
}
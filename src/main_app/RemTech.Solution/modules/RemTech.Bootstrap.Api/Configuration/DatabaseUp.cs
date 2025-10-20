using Cleaners.Module.Injection;
using RemTech.ContainedItems.Module.Injection;
using Shared.Infrastructure.Module.Postgres;

namespace RemTech.Bootstrap.Api.Configuration;

public static class DatabaseUp
{
    public static async Task UpDatabases(this WebApplication app)
    {
        IServiceProvider provider = app.Services;
        await UpDatabases(provider);
    }

    private static async Task UpDatabases(this IServiceProvider provider)
    {
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IEnumerable<IStorageUpper> uppers = scope.ServiceProvider.GetServices<IStorageUpper>();

        foreach (IStorageUpper upper in uppers)
            await upper.UpStorage();
    }
}

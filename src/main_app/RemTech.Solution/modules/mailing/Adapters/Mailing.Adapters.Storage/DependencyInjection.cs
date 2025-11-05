using Mailing.Adapters.Storage.Postmans;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Adapters.Storage;

public static class DependencyInjection
{
    public static void AddStorageAdapter(this IServiceCollection services)
    {
        services.AddUpgrader<DbPostman>(nameof(Mailing));
    }
}
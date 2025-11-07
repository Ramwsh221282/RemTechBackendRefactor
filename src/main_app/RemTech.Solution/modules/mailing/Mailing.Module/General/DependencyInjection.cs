using Mailing.Module.Infrastructure.NpgSql.Adapters.Storage;
using Mailing.Module.Infrastructure.NpgSql.Database;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Module.General;

public static class DependencyInjection
{
    public static void AddMailersModule(this IServiceCollection services)
    {
        services.AddNpgSqlAdapter();
    }

    private static void AddNpgSqlAdapter(this IServiceCollection services)
    {
        services.AddKeyedSingleton<IDatabaseUpgrader, MailingDbUpgrader>(nameof(Mailing));
        services.AddScoped<IMailersStorage, PgPostmans>();
    }
}
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Identity.Outbox;

public static class IdentityOutboxInjection
{
    public static void AddIdentityOutbox(this IServiceCollection services)
    {
        services.AddTransient<IDbUpgrader, IdentityOutboxDbUpgrader>();
    }
}
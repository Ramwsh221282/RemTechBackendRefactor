using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Tickets.Outbox;

public static class TicketOutboxDependencyInjection
{
    public static void AddTicketOutbox(this IServiceCollection services)
    {
        services.AddTransient<IDbUpgrader, TicketOutboxDbUpgrader>();
    }
}
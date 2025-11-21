using Microsoft.Extensions.Options;
using RemTech.NpgSql.Abstractions;

namespace Tickets.Outbox;

public sealed class TicketOutboxDbUpgrader : AbstractDatabaseUpgrader
{
    public TicketOutboxDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(TicketOutboxDbUpgrader).Assembly);
    }
}
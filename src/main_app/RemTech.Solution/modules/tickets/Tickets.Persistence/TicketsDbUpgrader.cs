using Microsoft.Extensions.Options;
using RemTech.NpgSql.Abstractions;

namespace Tickets.Persistence;

public sealed class TicketsDbUpgrader : AbstractDatabaseUpgrader
{
    public TicketsDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(TicketsDbUpgrader).Assembly);
    }
}
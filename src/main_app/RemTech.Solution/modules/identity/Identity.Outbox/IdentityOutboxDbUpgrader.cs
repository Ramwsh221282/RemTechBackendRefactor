using Microsoft.Extensions.Options;
using RemTech.NpgSql.Abstractions;

namespace Identity.Outbox;

public sealed class IdentityOutboxDbUpgrader : AbstractDatabaseUpgrader
{
    public IdentityOutboxDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(IdentityOutboxMessageModule).Assembly);
    }
}
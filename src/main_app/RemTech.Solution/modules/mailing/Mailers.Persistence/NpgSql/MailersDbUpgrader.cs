using Microsoft.Extensions.Options;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql;

public sealed class MailersDbUpgrader : AbstractDatabaseUpgrader
{
    public MailersDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(MailersDbUpgrader).Assembly);
    }
}
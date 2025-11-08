using Microsoft.Extensions.Options;

namespace RemTech.NpgSql.Abstractions;

public sealed class PgVectorUpgrader : AbstractDatabaseUpgrader
{
    public PgVectorUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(PgVectorUpgrader).Assembly);
    }
}
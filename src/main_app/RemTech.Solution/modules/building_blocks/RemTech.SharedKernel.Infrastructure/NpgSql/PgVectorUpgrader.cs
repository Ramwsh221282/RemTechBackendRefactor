using Microsoft.Extensions.Options;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public sealed class PgVectorUpgrader : AbstractDatabaseUpgrader
{
    public PgVectorUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(PgVectorUpgrader).Assembly);
    }
}
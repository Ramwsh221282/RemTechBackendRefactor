using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;

namespace Shared.Infrastructure.Module.Postgres;

internal sealed class PgVectorUpgrader : AbstractDatabaseUpgrader
{
    public PgVectorUpgrader(IOptions<DatabaseOptions> options) : base(options) =>
        OfAssembly(typeof(PgVectorUpgrader).Assembly);
}
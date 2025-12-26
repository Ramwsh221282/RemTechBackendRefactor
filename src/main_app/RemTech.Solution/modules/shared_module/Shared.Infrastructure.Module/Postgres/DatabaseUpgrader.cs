using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;

namespace Shared.Infrastructure.Module.Postgres;

public sealed class DatabaseUpgrader<TContext> : AbstractDatabaseUpgrader
{
    public DatabaseUpgrader(IOptions<DatabaseOptions> options) : base(options) =>
        OfAssembly(typeof(TContext).Assembly);
}
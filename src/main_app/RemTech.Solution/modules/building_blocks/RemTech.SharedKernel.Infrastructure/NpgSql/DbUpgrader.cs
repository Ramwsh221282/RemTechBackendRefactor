using Microsoft.Extensions.Options;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public sealed class DbUpgrader<T> : AbstractDatabaseUpgrader
{
    public DbUpgrader(IOptions<NpgSqlOptions> options) : base(options) =>
        OfAssembly(typeof(T).Assembly);
}
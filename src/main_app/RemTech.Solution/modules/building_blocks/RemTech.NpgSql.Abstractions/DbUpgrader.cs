using Microsoft.Extensions.Options;

namespace RemTech.NpgSql.Abstractions;

public sealed class DbUpgrader<T> : AbstractDatabaseUpgrader
{
    public DbUpgrader(IOptions<NpgSqlOptions> options) : base(options) =>
        OfAssembly(typeof(T).Assembly);
}
using System.Data.Common;

namespace Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

public sealed record ClosureNpgSqlReader(Func<Task<DbDataReader>> Reader)
{
    public ClosureNpgSqlReadingBlock<T> AsBlockOf<T>()
    {
        return new ClosureNpgSqlReadingBlock<T>(Reader);
    }
}

using System.Data.Common;

namespace Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

public sealed record ClosureNpgSqlReadingBlock<T>(Func<Task<DbDataReader>> Reader)
{
    private Func<DbDataReader, T>? _mapFn = null;

    public ClosureNpgSqlReadingBlock<T> Map(Func<DbDataReader, T> map)
    {
        _mapFn = map;
        return this;
    }

    public async Task<IEnumerable<T>> Results(CancellationToken ct = default)
    {
        if (_mapFn == null)
            return [];
        List<T> items = [];
        await using DbDataReader reader = await Reader();
        if (!reader.HasRows)
            return [];
        while (await reader.ReadAsync(ct))
        {
            items.Add(_mapFn(reader));
        }
        return items;
    }
}

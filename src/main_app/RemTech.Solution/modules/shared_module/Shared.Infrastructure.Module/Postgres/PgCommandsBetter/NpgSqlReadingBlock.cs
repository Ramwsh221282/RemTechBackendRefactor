using System.Data.Common;

namespace Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

public sealed record NpgSqlReadingBlock<T>(DbDataReader Reader) : IDisposable, IAsyncDisposable
{
    private IEnumerable<T> _items = [];

    public async Task<NpgSqlReadingBlock<T>> Map(
        Func<DbDataReader, T> func,
        CancellationToken ct = default
    )
    {
        if (!Reader.HasRows)
            return this;

        List<T> items = [];
        while (await Reader.ReadAsync(ct))
        {
            T item = func(Reader);
            items.Add(item);
        }

        _items = items;
        return this;
    }

    public IEnumerable<T> Result() => _items;

    public void Dispose()
    {
        Reader.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Reader.DisposeAsync();
    }
}

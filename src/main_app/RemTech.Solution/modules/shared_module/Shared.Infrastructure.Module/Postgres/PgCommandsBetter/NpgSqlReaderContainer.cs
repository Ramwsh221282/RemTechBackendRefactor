using System.Data.Common;

namespace Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

public sealed record NpgSqlReaderContainer(DbDataReader Reader) : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        Reader.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Reader.DisposeAsync();
    }

    public NpgSqlReadingBlock<T> AsBlockOf<T>()
    {
        return new NpgSqlReadingBlock<T>(Reader);
    }
}

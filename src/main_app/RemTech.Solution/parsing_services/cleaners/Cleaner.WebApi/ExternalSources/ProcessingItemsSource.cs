using Cleaner.WebApi.Models;
using Dapper;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaner.WebApi.ExternalSources;

public sealed class ProcessingItemsSource(PostgresDatabase database) : IProcessingItemsSource
{
    public async Task<IEnumerable<CleanerProcessingItem>> GetItemsForCleaner(
        WorkingCleaner cleaner,
        int batchSize
    )
    {
        var threshold = cleaner.ItemsDateDayThreshold;
        return await ReadItemsBatch(threshold, batchSize);
    }

    private async Task<IEnumerable<CleanerProcessingItem>> ReadItemsBatch(
        int threshold,
        int batchSize = 50
    )
    {
        const string sql = """
            SELECT
                id,
                type,
                domain,
                created_at,
                is_deleted,
                source_url
            FROM contained_items.items
            WHERE created_at > CURRENT_DATE - make_interval(days => @threshold)
              AND is_deleted = false
            ORDER BY id
            LIMIT @batchSize
            """;

        using var connection = await database.ProvideConnection();
        return await connection.QueryAsync<CleanerProcessingItem>(
            sql,
            new { threshold, batchSize }
        );
    }
}

using System.Data.Common;
using Cleaners.Module.Domain;
using Npgsql;

namespace Cleaners.Module.Database;

internal sealed class CleanerItemsSource(NpgsqlConnection connection)
{
    public async Task<CleanerItemsCollection> FetchByTreshold(
        ICleaner cleaner,
        CancellationToken ct = default
    )
    {
        CleanerItemsByTresholdVeil veil = cleaner
            .ProduceOutput()
            .PrintTo(new CleanerItemsByTresholdVeil());
        await using NpgsqlCommand command = connection.CreateCommand();
        veil.FillCommand(command);
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return new CleanerItemsCollection([]);
        List<CleanerItem> items = [];
        while (await reader.ReadAsync(ct))
        {
            string id = reader.GetString(0);
            string domain = reader.GetString(1);
            string sourceUrl = reader.GetString(2);
            items.Add(new CleanerItem(id, domain, sourceUrl));
        }
        return new CleanerItemsCollection(items);
    }
}

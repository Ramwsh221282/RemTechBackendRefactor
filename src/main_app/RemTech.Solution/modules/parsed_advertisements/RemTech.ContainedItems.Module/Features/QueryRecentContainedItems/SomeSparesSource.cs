using Npgsql;
using NpgsqlTypes;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal sealed class SomeSparesSource(NpgsqlConnection connection) : ISomeItemsSource
{
    private const string Sql = """
        SELECT
        object
        FROM spares_module.spares
        WHERE id = ANY(@ids)
        """;

    private const string Spare = "Запчасти";
    private const string IdsParam = "@ids";

    public async Task<IEnumerable<SomeRecentItem>> QueryItems(IEnumerable<QueriedRecentItem> items)
    {
        QueriedRecentItem[] array = items.ToArray();
        if (array.Length == 0 || array.Any(i => i.Type != Spare))
            return [];
        List<string> ids = new List<string>(array.Select(item => item.Id));
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(
            new NpgsqlParameter(IdsParam, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = ids.ToArray(),
            }
        );
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        List<RecentSpare> spares = [];
        while (await reader.ReadAsync())
        {
            spares.Add(new RecentSpare(reader));
        }
        return spares;
    }
}

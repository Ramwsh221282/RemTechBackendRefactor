using System.Data.Common;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal sealed class QueryRecentContainedItemsHandler(NpgsqlConnection connection)
    : ICommandHandler<QueryRecentContainedItemsCommand, IEnumerable<SomeRecentItem>>
{
    private const int PageSize = 10;
    private const int DateThreshHold = 3;
    private const string Spare = "Запчасти";
    private const string Vehicles = "Техника";

    private const string Sql = """
        SELECT id, type, created_at FROM contained_items.items
        WHERE is_deleted = FALSE 
        ORDER BY created_at DESC
        LIMIT @limit OFFSET @offset;
        """;
    private const string DateParam = "@dateThreshold";
    private const string LimitParam = "@limit";
    private const string OffsetParam = "@offset";
    private const string IdColumn = "id";
    private const string TypeColumn = "type";

    public async Task<IEnumerable<SomeRecentItem>> Handle(
        QueryRecentContainedItemsCommand command,
        CancellationToken ct = default
    )
    {
        IEnumerable<QueriedRecentItem> recentItem = await QueryRecentItems(command, ct);
        IEnumerable<QueriedRecentItem> spares = recentItem.Where(i => i.Type == Spare);
        IEnumerable<QueriedRecentItem> vehicles = recentItem.Where(i => i.Type == Vehicles);
        IEnumerable<SomeRecentItem> someSpares = await new SomeSparesSource(connection).QueryItems(
            spares
        );
        IEnumerable<SomeRecentItem> someVehicles = await new SomeVehiclesSource(
            connection
        ).QueryItems(vehicles);
        return [.. someVehicles, .. someSpares];
    }

    private async Task<IEnumerable<QueriedRecentItem>> QueryRecentItems(
        QueryRecentContainedItemsCommand command,
        CancellationToken ct = default
    )
    {
        if (command.Page <= 0)
            return [];
        DateTime now = DateTime.UtcNow;
        int offset = (command.Page - 1) * PageSize;
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<DateTime>(DateParam, now.AddDays(-DateThreshHold))
        );
        sqlCommand.Parameters.Add(new NpgsqlParameter<int>(LimitParam, PageSize));
        sqlCommand.Parameters.Add(new NpgsqlParameter<int>(OffsetParam, offset));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<QueriedRecentItem> items = [];
        while (await reader.ReadAsync(ct))
        {
            string id = reader.GetString(reader.GetOrdinal(IdColumn));
            string type = reader.GetString(reader.GetOrdinal(TypeColumn));
            items.Add(new QueriedRecentItem(id, type));
        }
        return items;
    }
}

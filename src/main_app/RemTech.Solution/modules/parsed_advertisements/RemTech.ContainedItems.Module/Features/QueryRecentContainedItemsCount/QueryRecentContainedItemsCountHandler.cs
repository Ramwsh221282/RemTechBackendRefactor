using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;

namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItemsCount;

internal sealed class QueryRecentContainedItemsCountHandler(NpgsqlConnection connection)
    : ICommandHandler<QueryRecentContainedItemsCountCommand, long>
{
    private const int DateThreshHold = 3;

    private const string Sql = """
        SELECT COUNT(id) as total_amount FROM contained_items.items
        WHERE created_at > @dateThreshold;
        """;
    private const string Param = "@dateThreshold";
    private const string Column = "total_amount";

    public async Task<long> Handle(
        QueryRecentContainedItemsCountCommand command,
        CancellationToken ct = default
    )
    {
        DateTime utcNow = DateTime.UtcNow;
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<DateTime>(Param, utcNow.AddDays(-DateThreshHold))
        );
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);
        return reader.GetInt64(reader.GetOrdinal(Column));
    }
}

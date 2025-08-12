using System.Data.Common;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.GetContainedVehiclesAmount;

internal sealed class GetContainedItemsByTypeHandler(NpgsqlConnection connection)
    : ICommandHandler<GetContainedItemsByTypeCommand, IEnumerable<GetContainedItemsByTypeResponse>>
{
    private const string Sql =
        "SELECT type, COUNT(id) as amount FROM contained_items.items WHERE is_deleted = FALSE GROUP BY type";
    private const string TypeColumn = "type";
    private const string AmountColumn = "amount";
    private const string K = "K";

    public async Task<IEnumerable<GetContainedItemsByTypeResponse>> Handle(
        GetContainedItemsByTypeCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        List<GetContainedItemsByTypeResponse> responses = [];
        while (await reader.ReadAsync(ct))
        {
            string type = reader.GetString(reader.GetOrdinal(TypeColumn));
            long amount = reader.GetInt64(reader.GetOrdinal(AmountColumn));
            string formatted = FormatCount(amount);
            responses.Add(new GetContainedItemsByTypeResponse(type, formatted));
        }
        return responses;
    }

    private static string FormatCount(long count)
    {
        string numberAsString = count.ToString();
        return numberAsString.Length > 4
            ? string.Concat(numberAsString.AsSpan(0, 1), K)
            : numberAsString;
    }
}

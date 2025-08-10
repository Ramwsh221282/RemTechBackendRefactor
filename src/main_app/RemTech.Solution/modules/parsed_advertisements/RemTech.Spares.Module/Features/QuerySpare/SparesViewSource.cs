using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Database.Embeddings;

namespace RemTech.Spares.Module.Features.QuerySpare;

internal sealed class SparesViewSource(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    Serilog.ILogger logger,
    SpareQuery query
)
{
    public async Task<IEnumerable<SpareQueryResult>> Read(CancellationToken ct = default)
    {
        if (query.Pagination.Page < 0)
            return [];
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        SparesSqlQuery sqlQuery = new(command, logger);
        if (query.Text != null && !string.IsNullOrWhiteSpace(query.Text.Text))
            sqlQuery.ApplyTextSearch(generator, query.Text.Text);
        sqlQuery.ApplyPagination(query.Pagination.Page);
        await using DbDataReader reader = await sqlQuery.Command().ExecuteReaderAsync(ct);
        List<SpareQueryResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            string objectData = reader.GetString(reader.GetOrdinal("object"));
            using SpareJsonObjectReader jsonReader = new(objectData);
            results.Add(jsonReader.Read());
        }
        return results;
    }
}

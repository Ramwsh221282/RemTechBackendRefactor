using System.Data.Common;
using System.Text;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.QueryBrands;

internal sealed class QueryBrandsHandler(NpgsqlConnection connection, IEmbeddingGenerator generator)
    : ICommandHandler<QueryBrandsCommand, IEnumerable<QueryBrandResult>>
{
    private const int PageSize = 30;

    private const string Sql = """
        SELECT b.id, b.name, COUNT(v.id)
        FROM parsed_advertisements_module.parsed_vehicles v
        LEFT JOIN brands_module.brands b ON b.id = v.brand_id
        GROUP BY b.id, b.name
        """;
    private const string EmbeddingParam = "@embedding";
    private const string EmbeddingOrdering = " ORDER BY b.embedding <=> @embedding ";
    private const string PaginationSqlPart = " LIMIT @limit OFFSET @offset ";
    private const string LimitParam = "@limit";
    private const string OffsetParam = "@offset";

    public async Task<IEnumerable<QueryBrandResult>> Handle(
        QueryBrandsCommand command,
        CancellationToken ct = default
    )
    {
        if (command.Page <= 0)
            return [];
        int offset = (command.Page - 1) * PageSize;
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        StringBuilder sqlBuilder = new StringBuilder(Sql);

        if (!string.IsNullOrWhiteSpace(command.Text))
        {
            sqlBuilder = sqlBuilder.AppendLine(EmbeddingOrdering);
            Vector vector = new Vector(generator.Generate(command.Text));
            sqlCommand.Parameters.AddWithValue(EmbeddingParam, vector);
        }

        sqlBuilder = sqlBuilder.AppendLine(PaginationSqlPart);
        sqlCommand.Parameters.AddWithValue(LimitParam, PageSize);
        sqlCommand.Parameters.AddWithValue(OffsetParam, offset);
        sqlCommand.CommandText = sqlBuilder.ToString();

        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<QueryBrandResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            string name = reader.GetString(1);
            long itemsCount = reader.GetInt64(2);
            results.Add(new QueryBrandResult(id, name, itemsCount));
        }
        return results;
    }
}

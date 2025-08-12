using System.Data.Common;
using System.Text;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.QueryBrandsAmount;

internal sealed class QueryBrandsAmountHandler(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<QueryBrandsAmountCommand, long>
{
    private const string Sql = """
        SELECT COUNT(DISTINCT b.id) AS amount
        FROM brands_module.brands b
        INNER JOIN parsed_advertisements_module.parsed_vehicles v ON b.id = v.brand_id
        """;

    private const string EmbeddingPart =
        "  GROUP BY b.embedding  ORDER BY b.embedding <=> @embedding  ";
    private const string EmbeddingParam = "@embedding";
    private const string AmountColumnName = "amount";

    public async Task<long> Handle(QueryBrandsAmountCommand command, CancellationToken ct = default)
    {
        StringBuilder queryBuilder = new(Sql);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        if (!string.IsNullOrWhiteSpace(command.Text))
        {
            queryBuilder = queryBuilder.AppendLine(EmbeddingPart);
            ReadOnlyMemory<float> embeddings = generator.Generate(command.Text);
            Vector vector = new Vector(embeddings);
            sqlCommand.Parameters.AddWithValue(EmbeddingParam, vector);
        }
        sqlCommand.CommandText = queryBuilder.ToString();
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);
        long amount = reader.GetInt64(reader.GetOrdinal(AmountColumnName));
        return amount;
    }
}

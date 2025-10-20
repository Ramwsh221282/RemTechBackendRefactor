using System.Data.Common;
using System.Text;
using Npgsql;
using Pgvector;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.QueryCategoriesAmount;

internal sealed class QueryCategoriesAmountHandler(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<QueryCategoriesAmountCommand, long>
{
    private const string Sql = """
        SELECT COUNT(DISTINCT c.id) AS amount
        FROM categories_module.categories c
        INNER JOIN parsed_advertisements_module.parsed_vehicles v ON c.id = v.kind_id
        """;

    private const string EmbeddingPart =
        "  GROUP BY c.embedding  ORDER BY c.embedding <=> @embedding  ";
    private const string EmbeddingParam = "@embedding";
    private const string AmountColumnName = "amount";

    public async Task<long> Handle(
        QueryCategoriesAmountCommand command,
        CancellationToken ct = default
    )
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

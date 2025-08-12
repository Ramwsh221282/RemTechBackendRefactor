using System.Data.Common;
using Categories.Module.Types;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.GetCategory;

internal sealed class GetCategoryByEmbeddingHandler(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<GetCategoryCommand, ICategory>
{
    private const string Sql = """
        SELECT id, name, rating FROM categories_module.categories
        ORDER BY embedding <=> @embedding
        LIMIT 1;
        """;

    private const string IdColumnName = "id";
    private const string NameColumnName = "name";
    private const string RatingColumnName = "rating";

    public async Task<ICategory> Handle(GetCategoryCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new GetCategoryByNameNameEmptyException();
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.AddWithValue(new Vector(generator.Generate(command.Name)));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new GetCategoryByNameNotFoundException(command.Name);
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal(IdColumnName));
        string name = reader.GetString(reader.GetOrdinal(NameColumnName));
        long rating = reader.GetInt64(reader.GetOrdinal(RatingColumnName));
        return new Category(id, name, rating);
    }
}

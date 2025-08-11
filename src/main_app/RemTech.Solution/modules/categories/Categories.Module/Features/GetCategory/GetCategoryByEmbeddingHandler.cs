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
    public async Task<ICategory> Handle(GetCategoryCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new GetCategoryByNameNameEmptyException();
        string sql = string.Intern(
            """
            SELECT id, name, rating FROM categories_module.categories
            ORDER BY embedding <=> @embedding
            LIMIT 1;
            """
        );
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.AddWithValue(new Vector(generator.Generate(command.Name)));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new GetCategoryByNameNotFoundException(command.Name);
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        long rating = reader.GetInt64(reader.GetOrdinal("rating"));
        return new Category(id, name, rating);
    }
}

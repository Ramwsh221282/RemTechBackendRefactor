using Categories.Module.Types;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.GetCategory;

internal sealed class GetNewlyCreatedCategory(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<GetCategoryCommand, ICategory>
{
    private const string Sql = """
        INSERT INTO categories_module.categories(id, name, rating, embedding)
        VALUES(@id, @name, @rating, @embedding)
        ON CONFLICT(name) DO NOTHING;
        """;
    private const string IdParam = "@id";
    private const string NameParam = "@name";
    private const string RatingParam = "@rating";
    private const string EmbeddingParam = "@embedding";

    public async Task<ICategory> Handle(GetCategoryCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new GetCategoryByNameNameEmptyException();
        ICategory newlyCreated = new Category(Guid.NewGuid(), command.Name, 0);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, newlyCreated.Id));
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>(NameParam, newlyCreated.Name));
        sqlCommand.Parameters.Add(new NpgsqlParameter<long>(RatingParam, newlyCreated.Rating));
        sqlCommand.Parameters.AddWithValue(
            EmbeddingParam,
            new Vector(generator.Generate(newlyCreated.Name))
        );
        int affected = await sqlCommand.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new GetNewlyCreatedCategoryDuplicateNameException(command.Name)
            : newlyCreated;
    }
}

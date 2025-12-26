using Models.Module.Types;
using Npgsql;
using Pgvector;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Models.Module.Features.GetModel;

internal sealed class GetNewlyCreatedModelHandler(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<GetModelCommand, IModel>
{
    private const string Sql = """
        INSERT INTO models_module.models(id, name, rating, embedding)
        VALUES(@id, @name, @rating, @embedding)
        ON CONFLICT(name) DO NOTHING;
        """;

    private const string IdParam = "@id";
    private const string NameParam = "@name";
    private const string RatingParam = "@rating";
    private const string EmbeddingParam = "@embedding";

    public async Task<IModel> Handle(GetModelCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new GetModelByNameNameEmptyException();
        IModel newModel = new Model(Guid.NewGuid(), command.Name, 0);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, newModel.Id));
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>(NameParam, newModel.Name));
        sqlCommand.Parameters.Add(new NpgsqlParameter<long>(RatingParam, newModel.Rating));
        sqlCommand.Parameters.AddWithValue(
            EmbeddingParam,
            new Vector(generator.Generate(newModel.Name))
        );
        int affected = await sqlCommand.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new GetNewlyCreatedModelDuplicateNameException(command.Name)
            : newModel;
    }
}

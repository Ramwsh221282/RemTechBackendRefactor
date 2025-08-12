using System.Data.Common;
using Models.Module.Types;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Models.Module.Features.GetModel;

internal sealed class GetModelByEmbeddingHandler(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<GetModelCommand, IModel>
{
    private const string Sql = """
        SELECT id, name, rating FROM models_module.models
        ORDER BY embedding <=> @embedding
        LIMIT 1;
        """;
    private const string EmbeddingParam = "@embedding";
    private const string IdColumn = "id";
    private const string NameColumn = "name";
    private const string RatingColumn = "rating";

    public async Task<IModel> Handle(GetModelCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(command.Name))
            throw new GetModelByNameNameEmptyException();
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.AddWithValue(
            EmbeddingParam,
            new Vector(generator.Generate(command.Name))
        );
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new GetModelByNameNotFoundException(command.Name);
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal(IdColumn));
        string name = reader.GetString(reader.GetOrdinal(NameColumn));
        long rating = reader.GetInt64(reader.GetOrdinal(RatingColumn));
        return new Model(id, name, rating);
    }
}

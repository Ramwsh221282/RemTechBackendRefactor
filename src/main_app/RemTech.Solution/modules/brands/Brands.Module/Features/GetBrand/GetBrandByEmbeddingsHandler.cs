using System.Data.Common;
using Brands.Module.Types;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.GetBrand;

internal sealed class GetBrandByEmbeddingsHandler(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<GetBrandCommand, IBrand>
{
    private const string Sql = """
        SELECT id, name, rating FROM brands_module.brands
        ORDER BY embedding <=> @embedding
        LIMIT 1;
        """;
    private const string IdColumn = "id";
    private const string NameColumn = "name";
    private const string RatingColumn = "rating";
    private const string EmbeddingParam = "@embedding";

    public async Task<IBrand> Handle(GetBrandCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new BrandRawByNameEmptyNameException();
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.AddWithValue(
            EmbeddingParam,
            new Vector(generator.Generate(command.Name))
        );
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new BrandRawByNameNotFoundException(command.Name);
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal(IdColumn));
        string name = reader.GetString(reader.GetOrdinal(NameColumn));
        long rating = reader.GetInt64(reader.GetOrdinal(RatingColumn));
        return new Brand(id, name, rating);
    }
}

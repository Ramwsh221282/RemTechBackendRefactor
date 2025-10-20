using Brands.Module.Types;
using Npgsql;
using Pgvector;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.GetBrand;

internal sealed class GetNewlyCreatedBrandHandler(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : ICommandHandler<GetBrandCommand, IBrand>
{
    private const string Sql = """
        INSERT INTO brands_module.brands(id, name, rating, embedding)
        VALUES(@id, @name, @rating, @embedding)
        ON CONFLICT(name) DO NOTHING;
        """;
    private const string IdParam = "@id";
    private const string NameParam = "@name";
    private const string RatingParam = "@rating";
    private const string EmbeddingParam = "@embedding";

    public async Task<IBrand> Handle(GetBrandCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new BrandRawByNameEmptyNameException();
        IBrand newBrand = new Brand(
            Guid.NewGuid(),
            command.Name == "PONSSE" ? "Ponsse" : command.Name,
            0
        );
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, newBrand.Id));
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>(NameParam, newBrand.Name));
        sqlCommand.Parameters.Add(new NpgsqlParameter<long>(RatingParam, newBrand.Rating));
        sqlCommand.Parameters.AddWithValue(
            EmbeddingParam,
            new Vector(generator.Generate(newBrand.Name))
        );
        int affected = await sqlCommand.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new GetNewlyCreatedBrandNameDuplicateException(command.Name)
            : newBrand;
    }
}

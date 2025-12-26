using System.Data;
using Brands.Module.Types;
using Dapper;
using Pgvector;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Features.GetBrand;

internal sealed class GetBrandHandler(PostgresDatabase database, IEmbeddingGenerator generator)
    : ICommandHandler<GetBrandCommand, Status<IBrand>>
{
    public async Task<Status<IBrand>> Handle(
        GetBrandCommand command,
        CancellationToken ct = default
    )
    {
        List<string> whereClauses = [];
        List<string> orderByClauses = [];
        DynamicParameters parameters = new();
        if (command.Id != null)
        {
            whereClauses.Add("id = @id");
            parameters.Add("@id", command.Id, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(command.Name))
        {
            whereClauses.Add("name = @name");
            parameters.Add("@name", command.Name, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(command.TextSearch))
        {
            Vector embedding = new(generator.Generate(command.TextSearch));
            orderByClauses.Add("embedding <=> @embedding DESC");
            parameters.Add("@embedding", embedding);
        }

        string whereClause =
            whereClauses.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", whereClauses);
        string orderByClause =
            orderByClauses.Count == 0
                ? string.Empty
                : " ORDER BY " + string.Join(", ", orderByClauses);

        string sql = $"""
            SELECT
            id,
            name,
            rating
            FROM brands_module.brands
            {whereClause}
            {orderByClause}
            LIMIT 1
            """;

        CommandDefinition sqlCommand = new(sql, parameters, cancellationToken: ct);
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        QueryiedBrand? brand = await connection.QueryFirstOrDefaultAsync<QueryiedBrand>(sqlCommand);
        return brand == null
            ? Error.NotFound("Бренд не найден.")
            : new Brand(brand.Id, brand.Name, brand.Rating);
    }

    private sealed class QueryiedBrand
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required long Rating { get; init; }
    }
}

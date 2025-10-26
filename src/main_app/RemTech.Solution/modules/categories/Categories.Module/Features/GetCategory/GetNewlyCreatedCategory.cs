using System.Data;
using Categories.Module.Types;
using Dapper;
using Pgvector;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.GetCategory;

internal sealed class GetCategoryHandler(PostgresDatabase database, IEmbeddingGenerator generator)
    : ICommandHandler<GetCategoryCommand, Status<ICategory>>
{
    public async Task<Status<ICategory>> Handle(
        GetCategoryCommand command,
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
            FROM categories_module.categories
            {whereClause}
            {orderByClause}
            LIMIT 1
            """;

        var sqlCommand = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = await database.ProvideConnection(ct: ct);
        var category = await connection.QueryFirstOrDefaultAsync<QueryiedCategory>(sqlCommand);
        return category == null
            ? Error.NotFound("Категория не найдена.")
            : new Category(category.Id, category.Name, category.Rating);
    }

    private sealed class QueryiedCategory
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required long Rating { get; init; }
    }
}

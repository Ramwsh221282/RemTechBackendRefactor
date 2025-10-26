using System.Data;
using Categories.Module.Responses;
using Dapper;
using Pgvector;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Features.QueryCategories;

internal sealed class QueryCategoriesHandler(
    PostgresDatabase database,
    IEmbeddingGenerator generator
) : ICommandHandler<QueryCategoriesCommand, QueryCategoriesResponse>
{
    public async Task<QueryCategoriesResponse> Handle(
        QueryCategoriesCommand command,
        CancellationToken ct = default
    )
    {
        List<string> whereClauses = [];
        List<string> orderByClauses = [];
        List<string> paginationClauses = [];
        DynamicParameters parameters = new();

        if (!string.IsNullOrWhiteSpace(command.Name))
        {
            whereClauses.Add("name ILIKE @name");
            parameters.Add("@name", $"%{command.Name}%", DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(command.TextSearch))
        {
            orderByClauses.Add("embedding <=> @embedding");
            Vector vector = new Vector(generator.Generate(command.TextSearch));
            parameters.Add("@embedding", vector);
        }

        orderByClauses.Add($"name {command.OrderMode}");
        int limit = command.PageSize;
        int offset = (command.Page - 1) * limit;
        paginationClauses.Add("LIMIT @limit");
        paginationClauses.Add("OFFSET @offset");
        parameters.Add("@limit", limit);
        parameters.Add("@offset", offset);

        string whereClause =
            whereClauses.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", whereClauses);
        string orderByClause =
            orderByClauses.Count == 0
                ? string.Empty
                : "ORDER BY " + string.Join(", ", orderByClauses);
        string paginationClause = string.Join(" ", paginationClauses);

        string sql = $"""
            SELECT 
                c.id,
                c.name,
                COUNT(*) OVER () AS count,
                (SELECT COUNT(*) FROM parsed_advertisements_module.parsed_vehicles v WHERE v.category_id = c.id) AS items_count
            FROM categories_module.categories c
            {whereClause}
            {orderByClause}
            {paginationClause}
            """;

        var sqlCommand = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = await database.ProvideConnection(ct);
        var data = await connection.QueryAsync<QueryCategoryModel>(sqlCommand);

        if (!data.Any())
            return new QueryCategoriesResponse(0, []);

        long count = data.First().Count;
        var brands = data.Select(d => new CategoryDto(d.Id, d.Name, d.ItemsCount));
        return new QueryCategoriesResponse(count, brands);
    }

    private sealed class QueryCategoryModel
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required long Count { get; init; }
        public required long ItemsCount { get; init; }
    }
}

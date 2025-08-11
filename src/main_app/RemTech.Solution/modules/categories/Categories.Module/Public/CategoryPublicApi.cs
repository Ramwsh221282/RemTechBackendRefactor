using Categories.Module.Features.GetCategory;
using Categories.Module.Types;
using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Categories.Module.Public;

internal sealed class CategoryPublicApi(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    Serilog.ILogger logger
) : ICategoryPublicApi
{
    public async Task<CategoryResponse> GetCategory(string name, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        GetCategoryVarietHandler handler = new GetCategoryVarietHandler(logger)
            .With(new GetCategoryByNameHandler(connection))
            .With(new GetCategoryByEmbeddingHandler(connection, generator));
        ICategory category = await handler.Handle(new GetCategoryCommand(name), ct);
        return new CategoryResponse(category.Id, name, category.Rating);
    }
}

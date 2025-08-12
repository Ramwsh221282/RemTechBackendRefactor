using Brands.Module.Features.GetBrand;
using Brands.Module.Types;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Brands.Module.Public;

internal sealed class BrandsPublicApi(
    IEmbeddingGenerator generator,
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger
) : IBrandsPublicApi
{
    public async Task<BrandResponse> GetBrand(string name, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        ICommandHandler<GetBrandCommand, IBrand> handler = new GetBrandVarietHandler(logger)
            .With(new GetNewlyCreatedBrandHandler(connection, generator))
            .With(new GetBrandByNameHandler(connection))
            .With(new GetBrandByEmbeddingsHandler(connection, generator));
        IBrand brand = await handler.Handle(new GetBrandCommand(name), ct);
        return new BrandResponse(brand.Name, brand.Id, brand.Rating);
    }
}

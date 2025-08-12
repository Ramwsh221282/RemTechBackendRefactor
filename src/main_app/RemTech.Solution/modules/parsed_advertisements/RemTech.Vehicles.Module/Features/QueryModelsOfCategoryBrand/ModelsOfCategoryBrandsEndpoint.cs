using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryModelsOfCategoryBrand;

internal static class ModelsOfCategoryBrandsEndpoint
{
    public static void Map(RouteGroupBuilder builder) =>
        builder.MapGet("category-brand-models", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] Guid categoryId,
        [FromQuery] Guid brandId,
        CancellationToken ct
    )
    {
        try
        {
            if (categoryId == Guid.Empty || brandId == Guid.Empty)
                return Results.Ok(Enumerable.Empty<ModelsOfCategoryBrandsResult>());
            ModelsOfCategoryBrandsQuery query = new ModelsOfCategoryBrandsQuery();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            IEnumerable<ModelsOfCategoryBrandsResult> results = await query.Query(
                categoryId,
                brandId,
                connection,
                ct
            );
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}", nameof(ModelsOfCategoryBrandsEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}

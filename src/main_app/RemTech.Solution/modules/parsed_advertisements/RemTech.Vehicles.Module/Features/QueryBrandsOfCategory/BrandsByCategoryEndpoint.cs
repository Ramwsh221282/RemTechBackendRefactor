using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryBrandsOfCategory;

public static class BrandsByCategoryEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("category-brands", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] Guid categoryId,
        CancellationToken ct
    )
    {
        try
        {
            if (categoryId == Guid.Empty)
                return Results.Ok(Enumerable.Empty<BrandsByCategoryResult>());
            BrandsByCategoryQuery query = new BrandsByCategoryQuery();
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            IEnumerable<BrandsByCategoryResult> result = await query.Query(
                categoryId,
                connection,
                ct
            );
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}", nameof(BrandsByCategoryEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения" });
        }
    }
}

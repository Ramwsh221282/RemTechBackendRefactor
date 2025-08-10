using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Database.Embeddings;

namespace RemTech.Spares.Module.Features.QuerySpare;

internal static class QuerySpareHttpEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] IEmbeddingGenerator generator,
        [FromServices] NpgsqlDataSource dataSource,
        [FromQuery] int page,
        [FromQuery] string? textSearch,
        CancellationToken ct
    )
    {
        try
        {
            SpareTextSearch? textSearchArgument = string.IsNullOrWhiteSpace(textSearch)
                ? null
                : new SpareTextSearch(textSearch);
            SpareQuery query = new SpareQuery(new SparePagination(page), textSearchArgument);
            SparesViewSource viewSource = new SparesViewSource(
                dataSource,
                generator,
                logger,
                query
            );
            IEnumerable<SpareQueryResult> results = await viewSource.Read(ct);
            logger.Information("Spares queried.");
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(QuerySpareHttpEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}

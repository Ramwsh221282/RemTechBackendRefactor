using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Persistance;

namespace Scrapers.Module.Domain.JournalsContext.Features.GetScraperJournalsCount;

public static class GetScraperJournalsCountEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("journals/count", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string name,
        [FromQuery] string type,
        CancellationToken ct
    )
    {
        try
        {
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            IScraperJournals journals = new NpgSqlScraperJournals(connection);
            long amount = await journals.GetCount(name, type, ct);
            return Results.Ok(amount);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}", nameof(GetScraperJournalsCountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}

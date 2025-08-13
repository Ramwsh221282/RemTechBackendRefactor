using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Spares.Module.Features.QuerySpare;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Spares.Module.Features.QuerySpareTotals;

public static class SparesCountEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("count", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] IEmbeddingGenerator generator,
        [FromQuery] string? textSearch,
        CancellationToken ct
    )
    {
        try
        {
            SpareTextSearch? textSearchArgument = string.IsNullOrWhiteSpace(textSearch)
                ? null
                : new SpareTextSearch(textSearch);
            SparesCountQuery query = new SparesCountQuery(textSearchArgument);
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            await using NpgsqlCommand command = connection.CreateCommand();
            SparesCountSqlQuery sqlQuery = new SparesCountSqlQuery(command);
            if (query.TextSearch != null && !string.IsNullOrWhiteSpace(query.TextSearch.Text))
                sqlQuery.ApplyTextSearch(generator, query.TextSearch.Text);
            object? count = await sqlQuery.Command().ExecuteScalarAsync(ct);
            long number = (long)count!;
            return Results.Ok(number);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}.", nameof(SparesCountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}

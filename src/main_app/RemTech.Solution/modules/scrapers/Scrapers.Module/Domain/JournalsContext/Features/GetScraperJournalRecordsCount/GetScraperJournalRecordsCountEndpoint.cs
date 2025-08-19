using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Persistance;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Scrapers.Module.Domain.JournalsContext.Features.GetScraperJournalRecordsCount;

public static class GetScraperJournalRecordsCountEndpoint
{
    public static void Map(RouteGroupBuilder builder) =>
        builder.MapGet("journals/records/count", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] IEmbeddingGenerator generator,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] Guid journalId,
        CancellationToken ct
    )
    {
        try
        {
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            IScraperJournalRecords records = new NpgSqlScraperJournalRecords(connection, generator);
            long amount = await records.GetCount(journalId, ct);
            return Results.Ok(amount);
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Entrance}. {Ex}.",
                nameof(GetScraperJournalRecordsCountEndpoint),
                ex.Message
            );
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}

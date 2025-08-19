using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Persistance;
using Scrapers.Module.Domain.JournalsContext.Responses;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Scrapers.Module.Domain.JournalsContext.Features.ReadScraperJournalRecords;

public static class ReadScraperJournalRecordsEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("journals/records", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] IEmbeddingGenerator generator,
        [FromQuery] Guid journalId,
        [FromQuery] int page,
        [FromQuery] string? text = null,
        CancellationToken ct = default
    )
    {
        try
        {
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            IScraperJournalRecords records = new NpgSqlScraperJournalRecords(connection, generator);
            IEnumerable<ScraperJournalRecord> recordsCollection = await records.GetPaged(
                journalId,
                page,
                text,
                ct
            );
            List<ScraperJournalRecordResponse> responses = [];
            foreach (ScraperJournalRecord record in recordsCollection)
            {
                ScraperJournalRecordResponseOutput output = new();
                output = record.PrintTo(output);
                await output.BehaveAsync();
                responses.Add(output.Read());
            }
            return Results.Ok(responses);
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Entrance}. {Ex}.",
                nameof(ReadScraperJournalRecordsEndpoint),
                ex.Message
            );
            return Results.BadRequest(new { message = "Ошибка на стороне приложения" });
        }
    }
}

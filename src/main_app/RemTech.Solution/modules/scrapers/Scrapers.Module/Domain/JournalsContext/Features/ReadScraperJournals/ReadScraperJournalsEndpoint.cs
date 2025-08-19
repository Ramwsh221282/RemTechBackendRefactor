using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Persistance;
using Scrapers.Module.Domain.JournalsContext.Responses;

namespace Scrapers.Module.Domain.JournalsContext.Features.ReadScraperJournals;

public static class ReadScraperJournalsEndpoint
{
    internal sealed record DateFilter(DateTime? From, DateTime? To);

    public static void Map(RouteGroupBuilder builder) => builder.MapPost("journals", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string name,
        [FromQuery] string type,
        [FromQuery] int page,
        [FromBody] DateFilter filter,
        CancellationToken ct = default
    )
    {
        try
        {
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            IScraperJournals journals = new NpgSqlScraperJournals(connection);
            IEnumerable<ScraperJournal> journalsCollection = await journals.GetPaged(
                name,
                type,
                page,
                filter.From,
                filter.To,
                ct
            );
            List<ScraperJournalResponse> responses = [];
            foreach (ScraperJournal journal in journalsCollection)
            {
                ScraperJournalResponseOutput output = new();
                output = journal.PrintTo(output);
                await output.BehaveAsync();
                responses.Add(output.Read());
            }
            return Results.Ok(responses);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(ReadScraperJournalsEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}

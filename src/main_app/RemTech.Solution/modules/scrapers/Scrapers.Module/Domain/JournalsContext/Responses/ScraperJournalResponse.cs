namespace Scrapers.Module.Domain.JournalsContext.Responses;

internal sealed record ScraperJournalResponse(
    Guid Id,
    string Name,
    string Type,
    DateTime CreatedAt,
    DateTime? CompletedAt
)
{
    public static ScraperJournalResponse Default() =>
        new(Guid.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue);
}

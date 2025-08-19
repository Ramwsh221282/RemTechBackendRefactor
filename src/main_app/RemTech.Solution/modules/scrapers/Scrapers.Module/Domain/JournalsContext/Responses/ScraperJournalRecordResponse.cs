namespace Scrapers.Module.Domain.JournalsContext.Responses;

internal sealed record ScraperJournalRecordResponse(
    Guid Id,
    Guid JournalId,
    string Action,
    string Text,
    DateTime CreatedAt
)
{
    public static ScraperJournalRecordResponse Default() =>
        new(Guid.Empty, Guid.Empty, string.Empty, string.Empty, DateTime.MinValue);
}

using Shared.Infrastructure.Module.Cqrs;

namespace Scrapers.Module.Domain.JournalsContext.Features.CompleteScraperJournal;

internal sealed record CompleteScraperJournalCommand(string ParserName, string ParserType)
    : ICommand;

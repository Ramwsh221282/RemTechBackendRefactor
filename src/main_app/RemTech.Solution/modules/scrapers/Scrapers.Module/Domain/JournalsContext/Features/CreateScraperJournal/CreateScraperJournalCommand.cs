using Shared.Infrastructure.Module.Cqrs;

namespace Scrapers.Module.Domain.JournalsContext.Features.CreateScraperJournal;

internal sealed record CreateScraperJournalCommand(string ScraperName, string ScraperType)
    : ICommand;

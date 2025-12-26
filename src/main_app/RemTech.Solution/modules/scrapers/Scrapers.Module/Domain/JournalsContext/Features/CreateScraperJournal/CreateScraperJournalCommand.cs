using RemTech.Core.Shared.Cqrs;

namespace Scrapers.Module.Domain.JournalsContext.Features.CreateScraperJournal;

internal sealed record CreateScraperJournalCommand(string ScraperName, string ScraperType)
    : ICommand;

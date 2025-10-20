using RemTech.Core.Shared.Cqrs;

namespace Scrapers.Module.Domain.JournalsContext.Features.RemoveScraperJournal;

internal sealed record RemoveScraperJournalCommand(Guid Id, string ScraperName, string ScraperType)
    : ICommand;

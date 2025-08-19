namespace Scrapers.Module.Domain.JournalsContext.Exceptions;

internal sealed class CannotRemoveNotCompletedScraperJournalException()
    : Exception(
        "Нельзя удалить журнал, пока парсер работает. Для удаления журнала парсер должен быть не в рабочем состоянии."
    );

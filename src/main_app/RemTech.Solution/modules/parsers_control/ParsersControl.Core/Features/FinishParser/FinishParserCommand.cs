using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.FinishParser;

/// <summary>
/// Команда завершения парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
/// <param name="TotalElapsedSeconds">Общее время выполнения парсера в секундах.</param>
public sealed record FinishParserCommand(Guid Id, long TotalElapsedSeconds) : ICommand;

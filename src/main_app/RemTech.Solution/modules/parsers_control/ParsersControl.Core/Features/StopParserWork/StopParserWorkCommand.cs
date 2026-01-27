using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.StopParserWork;

/// <summary>
/// Команда остановки работы парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
public sealed record StopParserWorkCommand(Guid Id) : ICommand;

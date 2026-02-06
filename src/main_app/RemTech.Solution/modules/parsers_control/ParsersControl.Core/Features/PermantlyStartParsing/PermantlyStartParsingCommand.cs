using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

/// <summary>
/// Команда постоянного запуска парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
public sealed record PermantlyStartParsingCommand(Guid Id) : ICommand;

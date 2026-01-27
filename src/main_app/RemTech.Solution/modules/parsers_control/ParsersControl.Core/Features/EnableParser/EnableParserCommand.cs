using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.EnableParser;

/// <summary>
/// Команда включения парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
public sealed record EnableParserCommand(Guid Id) : ICommand;

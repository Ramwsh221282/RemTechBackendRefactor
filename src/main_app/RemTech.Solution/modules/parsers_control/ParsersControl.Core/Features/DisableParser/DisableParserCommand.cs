using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.DisableParser;

/// <summary>
/// Команда отключения парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
public sealed record DisableParserCommand(Guid Id) : ICommand;

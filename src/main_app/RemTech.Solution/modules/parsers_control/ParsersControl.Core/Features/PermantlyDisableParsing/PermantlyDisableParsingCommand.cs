using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyDisableParsing;

/// <summary>
/// Команда постоянного отключения парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
public sealed record PermantlyDisableParsingCommand(Guid Id) : ICommand;

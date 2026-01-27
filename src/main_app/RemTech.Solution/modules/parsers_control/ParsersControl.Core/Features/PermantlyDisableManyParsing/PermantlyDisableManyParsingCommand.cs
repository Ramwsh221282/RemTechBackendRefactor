using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyDisableManyParsing;

/// <summary>
/// Команда постоянного отключения множества парсеров.
/// </summary>
/// <param name="Identifiers">Идентификаторы парсеров для отключения.</param>
public sealed record PermantlyDisableManyParsingCommand(IEnumerable<Guid> Identifiers) : ICommand;

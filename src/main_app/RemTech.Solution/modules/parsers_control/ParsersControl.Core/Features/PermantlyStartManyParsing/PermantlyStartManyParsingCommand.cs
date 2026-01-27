using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

/// <summary>
/// Команда постоянного запуска множества парсеров.
/// </summary>
/// <param name="Identifiers">Идентификаторы парсеров.</param>
public sealed record PermantlyStartManyParsingCommand(IEnumerable<Guid> Identifiers) : ICommand;

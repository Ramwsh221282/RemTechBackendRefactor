using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.StartParserWork;

/// <summary>
/// Команда начала работы парсера.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
public sealed record StartParserCommand(Guid Id) : ICommand;

using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SubscribeParser;

/// <summary>
/// Команда подписки на парсер.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
/// <param name="Domain">Домен парсера.</param>
/// <param name="Type">Тип парсера.</param>
public sealed record SubscribeParserCommand(Guid Id, string Domain, string Type) : ICommand;

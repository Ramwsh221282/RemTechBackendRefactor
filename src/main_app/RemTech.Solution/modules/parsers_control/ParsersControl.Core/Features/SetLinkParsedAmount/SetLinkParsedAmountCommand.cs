using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetLinkParsedAmount;

/// <summary>
/// Команда установки количества распарсенных ссылок.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="LinkId">Идентификатор ссылки.</param>
/// <param name="Amount">Количество распарсенных ссылок.</param>
public sealed record SetLinkParsedAmountCommand(Guid ParserId, Guid LinkId, int Amount) : ICommand;

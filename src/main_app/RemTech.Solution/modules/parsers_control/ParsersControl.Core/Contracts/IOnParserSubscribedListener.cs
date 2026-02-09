using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Core.Contracts;

/// <summary>
/// Слушатель события подписки на парсер.
/// </summary>
public interface IOnParserSubscribedListener
{
	/// <summary>
	/// Обрабатывает событие подписки на парсер.
	/// </summary>
	/// <param name="parser">Экземпляр подписанного парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача обработки события.</returns>
	Task Handle(SubscribedParser parser, CancellationToken ct = default);
}

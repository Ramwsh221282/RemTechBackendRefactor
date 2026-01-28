using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Core.Contracts;

/// <summary>
/// Слушатель события запуска парсера.
/// </summary>
public interface IOnParserStartedListener
{
	/// <summary>
	/// Обрабатывает событие запуска парсера.
	/// </summary>
	/// <param name="parser">Экземпляр запущенного парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача обработки события.</returns>
	Task Handle(SubscribedParser parser, CancellationToken ct = default);
}

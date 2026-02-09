using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

/// <summary>
/// Транспортировщик событий постоянного запуска парсера.
/// </summary>
/// <param name="listener">Слушатель событий запуска парсера.</param>
public sealed class OnPermantlyStartParsingEventTransporter(IOnParserStartedListener listener)
	: IEventTransporter<PermantlyStartParsingCommand, SubscribedParser>
{
	/// <summary>
	///  Транспортирует событие постоянного запуска парсера.
	/// </summary>
	/// <param name="result">Результат выполнения команды с запущенным парсером.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию транспортировки события.</returns>
	public Task Transport(SubscribedParser result, CancellationToken ct = default)
	{
		return listener.Handle(result, ct);
	}
}

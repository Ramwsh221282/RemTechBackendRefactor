using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

/// <summary>
/// Транспортировщик событий постоянного запуска множества парсеров.
/// </summary>
/// <param name="listener">Слушатель событий запуска парсеров.</param>
public sealed class OnPermantlyStartManyParsingEventTransporter(IOnParserStartedListener listener)
	: IEventTransporter<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>>
{
	/// <summary>
	///   Транспортирует событие постоянного запуска множества парсеров.
	/// </summary>
	/// <param name="result">Результат выполнения команды с запущенными парсерами.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию транспортировки события.</returns>
	public async Task Transport(IEnumerable<SubscribedParser> result, CancellationToken ct = default)
	{
		foreach (SubscribedParser parser in result)
		{
			await listener.Handle(parser, ct);
		}
	}
}

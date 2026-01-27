using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace ParsersControl.Core.Features.StartParserWork;

/// <summary>
/// Транспортировщик события начала работы парсера.
/// </summary>
/// <param name="listener">Слушатель события начала работы парсера.</param>
public sealed class ParserStartedEventTransporter(IOnParserStartedListener listener)
	: IEventTransporter<StartParserCommand, SubscribedParser>
{
	/// <summary>
	/// Транспортирует событие начала работы парсера.
	/// </summary>
	/// <param name="result">Результат события начала работы парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию транспортировки события.</returns>
	public Task Transport(SubscribedParser result, CancellationToken ct = default) => listener.Handle(result, ct);
}

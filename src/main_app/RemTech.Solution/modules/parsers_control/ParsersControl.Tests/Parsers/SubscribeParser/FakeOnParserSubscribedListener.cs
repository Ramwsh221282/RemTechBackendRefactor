using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Tests.Parsers.SubscribeParser;

/// <summary>
///   Фейковый слушатель события подписки на парсер.
/// </summary>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class FakeOnParserSubscribedListener(Serilog.ILogger logger) : IOnParserSubscribedListener
{
	/// <summary>
	///  Логгер для записи информации.
	/// </summary>
	public Serilog.ILogger Logger { get; } = logger.ForContext<IOnParserSubscribedListener>();

	/// <summary>
	/// Обрабатывает событие подписки на парсер.
	/// </summary>
	/// <param name="parser">Подписанный парсер.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public Task Handle(SubscribedParser parser, CancellationToken ct = default)
	{
		Logger.Information("Parser subscribed.");
		return Task.CompletedTask;
	}
}

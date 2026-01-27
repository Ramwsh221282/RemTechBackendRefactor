using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Tests.Parsers.SubscribeParser;

/// <summary>
///  Фейковый слушатель события начала работы парсера.
/// </summary>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class FakeOnParserWorkStartedListener(Serilog.ILogger logger) : IOnParserStartedListener
{
	/// <summary>
	/// Логгер для записи информации.
	/// </summary>
	public Serilog.ILogger Logger { get; } = logger.ForContext<IOnParserStartedListener>();

	/// <summary>
	/// Обрабатывает событие начала работы парсера.
	/// </summary>
	/// <param name="parser">Подписанный парсер.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public Task Handle(SubscribedParser parser, CancellationToken ct = default)
	{
		Logger.Information("Parser work started.");
		return Task.CompletedTask;
	}
}

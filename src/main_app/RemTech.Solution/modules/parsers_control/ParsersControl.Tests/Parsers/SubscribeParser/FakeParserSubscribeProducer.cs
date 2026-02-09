using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Tests.Parsers.SubscribeParser;

/// <summary>
/// Фейковый продюсер сообщений подписки на парсер.
/// </summary>
/// <param name="logger">Логгер для записи информации.</param>
/// <param name="producer">Продюсер RabbitMQ для отправки сообщений.</param>
public sealed class FakeParserSubscribeProducer(Serilog.ILogger logger, RabbitMqProducer producer)
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<FakeParserSubscribeProducer>();

	/// <summary>
	/// Публикует сообщение подписки на парсер.
	/// </summary>
	/// <param name="parserId">Идентификатор парсера.</param>
	/// <param name="domain">Домен парсера.</param>
	/// <param name="type">Тип парсера.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task Publish(Guid parserId, string domain, string type)
	{
		Logger.Information(
			"Publishing parser subscribe message. Id: {Id}, Domain: {Domain}, Type: {Type}",
			parserId,
			domain,
			type
		);
		SubscribeParserMessage message = new()
		{
			parser_id = parserId,
			parser_domain = domain,
			parser_type = type,
		};

		await producer.PublishDirectAsync(message, "parsers", "parsers.create", new RabbitMqPublishOptions());
		Logger.Information("Published parser subscribe message.");
	}

	/// <summary>
	/// Сообщение подписки на парсер.
	/// </summary>
	public sealed class SubscribeParserMessage
	{
		/// <summary>
		/// Идентификатор парсера.
		/// </summary>
		public Guid parser_id { get; set; }

		/// <summary>
		/// Домен парсера.
		/// </summary>
		public string parser_domain { get; set; } = null!;

		/// <summary>
		/// Тип парсера.
		/// </summary>
		public string parser_type { get; set; } = null!;
	}
}

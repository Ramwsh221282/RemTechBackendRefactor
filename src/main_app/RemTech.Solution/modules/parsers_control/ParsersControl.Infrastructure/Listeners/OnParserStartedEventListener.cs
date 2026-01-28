using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.Listeners;

/// <summary>
/// Обработчик события начала парсера.
/// </summary>
/// <param name="producer">Производитель сообщений RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class OnParserStartedEventListener(RabbitMqProducer producer, Serilog.ILogger logger)
	: IOnParserStartedListener
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<IOnParserStartedListener>();

	/// <summary>
	/// Обрабатывает событие начала парсера.
	/// </summary>
	/// <param name="parser">Подписанный парсер, для которого произошло событие начала.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию обработки события.</returns>
	public Task Handle(SubscribedParser parser, CancellationToken ct = default)
	{
		Logger.Information("Handling on parser started event.");
		(Guid parserId, string domain, string type, IReadOnlyList<SubscribedParserLink> links) = GetParserInfo(parser);
		(string exchange, string routingKey) = FormPublishingOptions(domain, type);
		Logger.Information("Domain: {Domain}, Type: {Type}, Id: {Id}", domain, type, parserId);
		StartParserMessage body = CreatePayload(parserId, domain, type, links);
		return PublishMessage(exchange, routingKey, body, ct);
	}

	private static (Guid ParserId, string Domain, string Type, IReadOnlyList<SubscribedParserLink> Links) GetParserInfo(
		SubscribedParser parser
	)
	{
		string parserDomain = parser.Identity.DomainName;
		string parserType = parser.Identity.ServiceType;
		Guid parserId = parser.Id.Value;
		IReadOnlyList<SubscribedParserLink> links = parser.Links;
		return (parserId, parserDomain, parserType, links);
	}

	private static (string Exchange, string RoutingKey) FormPublishingOptions(string domain, string type)
	{
		string exchange = $"{domain}.{type}";
		string routingKey = $"{exchange}.start";
		return (exchange, routingKey);
	}

	private static StartParserMessage CreatePayload(
		Guid parserId,
		string domain,
		string type,
		IReadOnlyList<SubscribedParserLink> links
	) =>
		new()
		{
			parser_id = parserId,
			parser_domain = domain,
			parser_type = type,
			parser_links = [.. links.Select(l => new StartParserMessageLinks { id = l.Id.Value, url = l.UrlInfo.Url })],
		};

	private async Task PublishMessage(
		string exchange,
		string routingKey,
		StartParserMessage body,
		CancellationToken ct = default
	)
	{
		await producer.PublishDirectAsync(
			body,
			exchange,
			routingKey,
			new RabbitMqPublishOptions() { Persistent = true },
			ct
		);
		Logger.Information("Published message to exchange {Exchange}, routing key {RoutingKey}", exchange, routingKey);
	}

	private sealed class StartParserMessage
	{
		public Guid parser_id { get; set; }
		public required string parser_domain { get; set; }
		public required string parser_type { get; set; }
		public required StartParserMessageLinks[] parser_links { get; set; }
	}

	private sealed class StartParserMessageLinks
	{
		public required Guid id { get; set; }
		public required string url { get; set; }
	}
}

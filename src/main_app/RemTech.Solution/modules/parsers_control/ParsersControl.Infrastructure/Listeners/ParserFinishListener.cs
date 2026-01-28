using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.FinishParser;
using ParsersControl.Core.Parsers.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.Listeners;

/// <summary>
/// Обработчик события завершения парсера.
/// </summary>
/// <param name="serviceProvider">Поставщик сервисов для создания областей обслуживания.</param>
/// <param name="rabbitMq">Источник подключения к RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class ParserFinishListener(
	IServiceProvider serviceProvider,
	RabbitMqConnectionSource rabbitMq,
	Serilog.ILogger logger
) : IConsumer
{
	private const string EXCHANGE = "parsers";
	private const string QUEUE = "parsers.finish";
	private const string ROUTING_KEY = QUEUE;

	private IChannel? _channel;
	private Serilog.ILogger Logger { get; } = logger.ForContext<ParserFinishListener>();
	private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel is not initialized.");

	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, ea) =>
		{
			Logger.Information("Received message. {Queue} {Exchange} {RoutingKey}", QUEUE, EXCHANGE, ROUTING_KEY);
			try
			{
				await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
				ICommandHandler<FinishParserCommand, SubscribedParser> handler =
					scope.ServiceProvider.GetRequiredService<ICommandHandler<FinishParserCommand, SubscribedParser>>();
				ParserFinishMessage message = ParserFinishMessage.FromEvent(ea);
				FinishParserCommand command = new(message.Id, message.TotalElapsedSeconds);
				Result<SubscribedParser> result = await handler.Execute(command);
				if (result.IsFailure)
				{
					Logger.Error("Error at finishing parser. {Error}", result.Error.Message);
				}

				await Channel.BasicAckAsync(ea.DeliveryTag, false);
			}
			catch (Exception e)
			{
				Logger.Error(
					e,
					"Error processing message. {Queue} {Exchange} {RoutingKey}",
					QUEUE,
					EXCHANGE,
					ROUTING_KEY
				);
				await Channel.BasicNackAsync(ea.DeliveryTag, false, false);
			}
		};

	/// <summary>
	/// Инициализирует канал для прослушивания сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию инициализации канала.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default) =>
		_channel = await TopicConsumerInitialization.InitializeChannel(rabbitMq, EXCHANGE, QUEUE, ROUTING_KEY, ct);

	/// <summary>
	/// Начинает прослушивание сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию начала прослушивания сообщений.</returns>
	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(QUEUE, false, consumer, cancellationToken: ct);
	}

	/// <summary>
	/// Останавливает прослушивание сообщений и закрывает канал.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию остановки прослушивания сообщений.</returns>
	public Task Shutdown(CancellationToken ct = default) => Channel.CloseAsync(ct);

	private sealed class ParserFinishMessage
	{
		public Guid Id { get; set; }
		public long TotalElapsedSeconds { get; set; }

		public static ParserFinishMessage FromEvent(BasicDeliverEventArgs ea) =>
			JsonSerializer.Deserialize<ParserFinishMessage>(ea.Body.Span)!;
	}
}

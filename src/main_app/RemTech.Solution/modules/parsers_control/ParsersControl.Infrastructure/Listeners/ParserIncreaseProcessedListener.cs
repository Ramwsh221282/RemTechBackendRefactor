using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.SetParsedAmount;
using ParsersControl.Core.Parsers.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.Listeners;

/// <summary>
/// Слушатель увеличения количества обработанных элементов парсером.
/// </summary>
/// <param name="rabbitMq">Источник подключения к RabbitMQ.</param>
/// <param name="logger">Логгер для записи информации и ошибок.</param>
/// <param name="services">Провайдер сервисов для разрешения зависимостей.</param>
public sealed class ParserIncreaseProcessedListener(
	RabbitMqConnectionSource rabbitMq,
	Serilog.ILogger logger,
	IServiceProvider services
) : IConsumer
{
	private const string EXCHANGE = "parsers";
	private const string QUEUE = "parsers.increase_processed";
	private const string ROUTING_KEY = "parsers.increase_processed";
	private IChannel? _channel;
	private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
	private Serilog.ILogger Logger { get; } = logger.ForContext<ParserIncreaseProcessedListener>();
	private IServiceProvider Services { get; } = services;
	private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel was not initialized.");
	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, @event) =>
		{
			Logger.Information("Handling message.");
			try
			{
				ParserIncreaseProcessedMessage message = ParserIncreaseProcessedMessage.CreateFrom(@event);
				if (!IsMessageValid(message, out string error))
				{
					Logger.Error("Denied message: {Error}", error);
					return;
				}

				SetParsedAmountCommand command = CreateCommand(message);
				Result result = await HandleCommand(command);
				if (result.IsFailure)
					Logger.Error("Error handling command: {Error}", result.Error.Message);
				Logger.Information("Message handled.");
			}
			catch (Exception e)
			{
				Logger.Fatal(e, "Error handling message.");
			}
			finally
			{
				await Channel.BasicAckAsync(@event.DeliveryTag, false);
			}
		};

	/// <summary>
	/// Инициализирует канал для прослушивания сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Инициализация канала.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default) =>
		_channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, EXCHANGE, QUEUE, ROUTING_KEY, ct);

	/// <summary>
	/// Начинает прослушивание сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача прослушивания сообщений.</returns>
	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(QUEUE, false, consumer, ct);
	}

	/// <summary>
	/// Останавливает прослушивание сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача остановки прослушивания сообщений.</returns>
	public Task Shutdown(CancellationToken ct = default) => Channel.CloseAsync(ct);

	private static SetParsedAmountCommand CreateCommand(ParserIncreaseProcessedMessage message) =>
		new(message.Id, message.Amount);

	private static bool IsMessageValid(ParserIncreaseProcessedMessage message, out string error)
	{
		List<string> errors = [];
		if (message.Id == Guid.Empty)
			errors.Add("Идентификатор парсера пуст");
		if (message.Amount == 0)
			errors.Add("Количество обработанных пуст");
		error = string.Join(", ", errors);
		return errors.Count == 0;
	}

	private async Task<Result> HandleCommand(SetParsedAmountCommand command)
	{
		await using AsyncServiceScope scope = Services.CreateAsyncScope();
		return await scope
			.ServiceProvider.GetRequiredService<ICommandHandler<SetParsedAmountCommand, SubscribedParser>>()
			.Execute(command);
	}

	private sealed class ParserIncreaseProcessedMessage
	{
		public Guid Id { get; set; }
		public int Amount { get; set; }

		public static ParserIncreaseProcessedMessage CreateFrom(BasicDeliverEventArgs @event) =>
			JsonSerializer.Deserialize<ParserIncreaseProcessedMessage>(@event.Body.Span)!;
	}
}

using System.Text.Json;
using Microsoft.Extensions.Options;
using Notifications.Core.PendingEmails.Features.AddPendingEmail;
using Notifications.Infrastructure.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Notifications.Infrastructure.RabbitMq.Consumers;

/// <summary>
/// Потребитель сообщений о создании новой учетной записи.
/// </summary>
/// <param name="rabbitMq">Источник подключения к RabbitMQ.</param>
/// <param name="services">Поставщик сервисов.</param>
/// <param name="logger">Логгер для записи событий.</param>
/// <param name="frontendOptions">Настройки фронтенда.</param>
public sealed class OnNewAccountCreatedConsumer(
	RabbitMqConnectionSource rabbitMq,
	IServiceProvider services,
	Serilog.ILogger logger,
	IOptions<FrontendOptions> frontendOptions
) : IConsumer
{
	private const string EXCHANGE = "identity";
	private const string ROUTING_KEY = "account.new";
	private const string QUEUE = "account.new";

	private IChannel? _channel;
	private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel was not initialized.");
	private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
	private IServiceProvider Services { get; } = services;
	private Serilog.ILogger Logger { get; } = logger.ForContext<OnNewAccountCreatedConsumer>();
	private FrontendOptions FrontendOptions { get; } = frontendOptions.Value;
	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, @event) =>
		{
			Logger.Information("Received message: {Message}", @event.Body.ToArray());
			try
			{
				NewAccountRegisteredOutboxMessagePayload payload =
					NewAccountRegisteredOutboxMessagePayload.FromDeliverEventArgs(@event);

				if (!payload.IsValid(out string error))
				{
					Logger.Warning("Invalid message: {Error}", error);
					return;
				}

				Result<Unit> result = await HandleMessage(payload);
				if (result.IsFailure)
				{
					Logger.Warning(result.Error, "Error processing message: {Message}", result.Error.Message);
				}
				else
				{
					Logger.Information("Processed message: {Message}", @event.Body.ToArray());
				}
			}
			catch (Exception e)
			{
				Logger.Fatal(e, "Error processing message: {Message}", @event.Body.ToArray());
			}
			finally
			{
				await Channel.BasicAckAsync(@event.DeliveryTag, false);
			}
		};

	/// <summary>
	/// Инициализирует канал для потребления сообщений.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инициализации канала.</returns>
	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
	{
		FrontendOptions.Validate();
		_channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, EXCHANGE, QUEUE, ROUTING_KEY, ct);
	}

	/// <summary>
	/// Начинает потребление сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию начала потребления сообщений.</returns>
	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(QUEUE, false, consumer, ct);
	}

	/// <summary>
	/// Завершает работу потребителя.
	/// </summary>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию завершения работы потребителя.</returns>
	public async Task Shutdown(CancellationToken ct = default)
	{
		await Channel.CloseAsync(ct);
		await Channel.DisposeAsync();
	}

	private Task<Result<Unit>> HandleMessage(NewAccountRegisteredOutboxMessagePayload payload)
	{
		string confirmationLink = BuildConfirmationLinkUrl(payload);
		AddPendingEmailCommand command = new(
			payload.Email,
			"Подтверждение почты",
			$"Для подтверждения почты перейдите по ссылке {confirmationLink}"
		);
		return Services.CreatePendingMessage(command);
	}

	private string BuildConfirmationLinkUrl(NewAccountRegisteredOutboxMessagePayload payload)
	{
		FrontendOptions.Validate();
		string frontendUrl = FrontendOptions.Url;
		string ticketId = payload.TicketId.ToString();
		return $"{frontendUrl}/sign-up/confirmation?ticketId={ticketId}&accountId={payload.AccountId}";
	}

	private sealed record NewAccountRegisteredOutboxMessagePayload(
		Guid AccountId,
		Guid TicketId,
		string Email,
		string Login
	)
	{
		public static NewAccountRegisteredOutboxMessagePayload FromDeliverEventArgs(BasicDeliverEventArgs @event)
		{
			return JsonSerializer.Deserialize<NewAccountRegisteredOutboxMessagePayload>(@event.Body.Span)!;
		}

		public bool IsValid(out string error)
		{
			List<string> errors = [];
			if (AccountId == Guid.Empty)
			{
				errors.Add("Идентификатор аккаунта пуст");
			}

			if (TicketId == Guid.Empty)
			{
				errors.Add("Идентификатор тикета пуст");
			}

			if (string.IsNullOrWhiteSpace(Email))
			{
				errors.Add("Почта пуста");
			}
			if (string.IsNullOrWhiteSpace(Login))
			{
				errors.Add("Логин пуст");
			}

			error = string.Join(", ", errors);
			return errors.Count == 0;
		}
	}
}

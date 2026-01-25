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

public sealed class ParserFinishListener(
	IServiceProvider serviceProvider,
	RabbitMqConnectionSource rabbitMq,
	Serilog.ILogger logger
) : IConsumer
{
	private const string Exchange = "parsers";
	private const string Queue = "parsers.finish";
	private const string RoutingKey = Queue;

	private IChannel? _channel;
	private Serilog.ILogger Logger { get; } = logger.ForContext<ParserFinishListener>();
	private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel is not initialized.");

	private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
		async (_, ea) =>
		{
			Logger.Information("Received message. {Queue} {Exchange} {RoutingKey}", Queue, Exchange, RoutingKey);
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
					Queue,
					Exchange,
					RoutingKey
				);
				await Channel.BasicNackAsync(ea.DeliveryTag, false, false);
			}
		};

	public async Task InitializeChannel(IConnection connection, CancellationToken ct = default) =>
		_channel = await TopicConsumerInitialization.InitializeChannel(rabbitMq, Exchange, Queue, RoutingKey, ct);

	public Task StartConsuming(CancellationToken ct = default)
	{
		AsyncEventingBasicConsumer consumer = new(Channel);
		consumer.ReceivedAsync += Handler;
		return Channel.BasicConsumeAsync(Queue, false, consumer, cancellationToken: ct);
	}

	public Task Shutdown(CancellationToken ct = default) => Channel.CloseAsync(ct);

	private sealed class ParserFinishMessage
	{
		public Guid Id { get; set; }
		public long TotalElapsedSeconds { get; set; }

		public static ParserFinishMessage FromEvent(BasicDeliverEventArgs ea) =>
			JsonSerializer.Deserialize<ParserFinishMessage>(ea.Body.Span)!;
	}
}

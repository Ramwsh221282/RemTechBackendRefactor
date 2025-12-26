using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.SubscribeParser;
using ParsersControl.Core.Parsers.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.Listeners;

public sealed class ParserSubscribeConsumer(IServiceProvider services, Serilog.ILogger logger) : IConsumer
{
    private const string Queue = "parsers.create";
    private const string Exchange = "parsers";
    private const string RoutingKey = Queue;

    private Serilog.ILogger Logger { get; } = logger.ForContext<ParserSubscribeConsumer>();
    private IChannel? Channel { get; set; }
    private AsyncEventingBasicConsumer? Consumer { get; set; }

    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
    {
        Logger.Information("Initializing channel.");
        
        CreateChannelOptions options = new(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);

        Channel = await connection.CreateChannelAsync(options, cancellationToken: ct);
        await DeclareExchange(Channel, ct);
        await DeclareQueue(Channel, ct);
        await BindQueue(Channel, ct);
        Consumer = CreateConsumer(Channel);
        
        Logger.Information("Initialized channel.");
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        if (Channel is null) throw new InvalidOperationException("Channel is not initialized.");
        if (Consumer is null) throw new InvalidOperationException("Consumer is not initialized.");
        await Channel.BasicConsumeAsync(
            queue: Queue,
            autoAck: true,
            consumer: Consumer,
            cancellationToken: ct);
    }

    public async Task Shutdown(CancellationToken ct = default)
    {
        if (Channel is null) throw new InvalidOperationException("Channel is not initialized.");
        Logger.Information("Shutting down channel.");
        await Channel.DisposeAsync();
        Logger.Information("Channel shut down.");
    }

    private static async Task DeclareExchange(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(
            exchange: Exchange,
            type: "topic",
            durable: true,
            autoDelete: false,
            cancellationToken: ct);
    }
    
    private static async Task DeclareQueue(IChannel channel, CancellationToken ct)
    {
        await channel.QueueDeclareAsync(
            queue: Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);
    }
    
    private static async Task BindQueue(IChannel channel, CancellationToken ct)
    {
        await channel.QueueBindAsync(
            queue: Queue,
            exchange: Exchange,
            routingKey: RoutingKey,
            cancellationToken: ct);
    }

    private AsyncEventingBasicConsumer CreateConsumer(IChannel channel)
    {
        if (Channel is null) throw new InvalidOperationException("Channel is not initialized.");
        
        AsyncEventingBasicConsumer consumer = new(channel);
        consumer.ReceivedAsync += async (_, @event) =>
        {
            Logger.Information("Handling message.");
            try
            {
                SubscribeParserMessage message = SubscribeParserMessage.Create(@event);
                await using AsyncServiceScope scope = services.CreateAsyncScope();
                ICommandHandler<SubscribeParserCommand, SubscribedParser> handler =
                    scope.ServiceProvider
                        .GetRequiredService<ICommandHandler<SubscribeParserCommand, SubscribedParser>>();
                SubscribeParserCommand command = new(message.parser_id, message.parser_domain, message.parser_type);
                await handler.Execute(command);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Failed to process message. {Ex}", ex);
            }
            finally
            {
                await Channel.BasicAckAsync(@event.DeliveryTag, false);
            }
        };
        
        return consumer;
    }
    
    private sealed class SubscribeParserMessage
    {
        public Guid parser_id { get; set; }
        public string parser_domain { get; set; }
        public string parser_type { get; set; }

        public static SubscribeParserMessage Create(BasicDeliverEventArgs @event)
        {
            return JsonSerializer.Deserialize<SubscribeParserMessage>(@event.Body.Span)!;
        }
    }
}
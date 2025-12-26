using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ParsersControl.Core.Features.SubscribeParser;
using ParsersControl.Core.Parsers.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.RabbitMQ.Listeners;

public sealed class ParserSubscribeListener(
    RabbitMqConnectionSource rabbitMq,
    Serilog.ILogger logger,
    IServiceProvider sp) : BackgroundService
{
    private const string Queue = "parsers.create";
    private const string Exchange = "parsers";
    private const string RoutingKey = Queue;
    private IChannel Channel { get; set; } = null!;
    private Serilog.ILogger Logger { get; } = logger.ForContext<ParserSubscribeListener>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeChannel(stoppingToken);
        await SetupExchange(stoppingToken);
        await SetupQueue(stoppingToken);
        AsyncEventingBasicConsumer consumer = SetupConsumer(stoppingToken);
        await Channel.BasicConsumeAsync(
            queue: Queue,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken
        );
    }

    private async Task InitializeChannel(CancellationToken ct)
    {
        IConnection connection = await rabbitMq.GetConnection(ct);
        CreateChannelOptions options = new(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true
            );
        Channel = await connection.CreateChannelAsync(options, ct);
    }

    private async Task SetupExchange(CancellationToken ct)
    {
        await Channel.ExchangeDeclareAsync(
            Exchange,
            "topic",
            durable: true,
            autoDelete: false);
        Logger.Information("Exchange {Exchange} declared.", Exchange);
    }

    private async Task SetupQueue(CancellationToken ct)
    {
        await Channel.QueueDeclareAsync(
            queue: Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct
        );
        
        Logger.Information("Queue {Queue} declared.", Queue);
        
        await Channel.QueueBindAsync(
            queue: Queue,
            exchange: Exchange,
            routingKey: RoutingKey,
            cancellationToken: ct
        );
        
        Logger.Information("Queue {Queue} bound to exchange {Exchange} with routing key {RoutingKey}.", Queue, Exchange, RoutingKey);
    }
    
    private AsyncEventingBasicConsumer SetupConsumer(CancellationToken ct)
    {
        AsyncEventingBasicConsumer consumer = new(Channel);
        consumer.ReceivedAsync += Handler;
        
        Logger.Information("Consumer for queue {Queue} created.", Queue);
        
        return consumer;
    }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler => async (sender, ea) =>
    {
        logger.Information("Received message in parser subscribe listener.");
        try
        {
            (Guid id, string domain, string type) = ParsePayload(ea);
            logger.Information("Subscribing parser with: {Id} {Domain} {Type}", id, domain, type);
            await InvokeCommand(sp, (id, domain, type));
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}", nameof(Handler), ex);
        }
        finally
        {
            await Channel.BasicAckAsync(ea.DeliveryTag, false);
        }
    };

    private static (Guid id, string domain, string type) ParsePayload(BasicDeliverEventArgs ea)
    {
        using JsonDocument document = JsonDocument.Parse(ea.Body.ToArray());
        Guid parserId = document.RootElement.GetProperty("parser_id").GetGuid();
        string parserType = document.RootElement.GetProperty("parser_type").GetString()!;
        string parserDomain = document.RootElement.GetProperty("parser_domain").GetString()!;
        return (parserId, parserDomain, parserType);
    }

    private static async Task InvokeCommand(IServiceProvider sp, (Guid id, string domain, string type) payload)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        
        ICommandHandler<SubscribeParserCommand, SubscribedParser> handler =
            scope.ServiceProvider.GetRequiredService<ICommandHandler<SubscribeParserCommand, SubscribedParser>>();
        
        SubscribeParserCommand command = new(payload.id, payload.domain, payload.type);
        await handler.Execute(command);
    }
}




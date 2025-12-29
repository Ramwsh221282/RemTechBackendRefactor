using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Spares.Domain.Features;

namespace Spares.Infrastructure.Consumers;

public sealed class AddSparesConsumer(
    RabbitMqConnectionSource rabbitMq,
    Serilog.ILogger logger,
    IServiceProvider services) : IConsumer
{
    private const string Exchange = "spares";
    private const string Queue = "spares.add";
    private const string RoutingKey = "spares.add";

    private IChannel? _channel;
    private IChannel Channel => _channel ?? throw new InvalidOperationException();
    private Serilog.ILogger Logger { get; } = logger.ForContext<AddSparesConsumer>();
    private IServiceProvider Services { get; } = services;
    private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
    
    public async Task InitializeChannel(IConnection connection, CancellationToken ct = new CancellationToken())
    {
        _channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, Exchange, Queue, RoutingKey, ct);
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        AsyncEventingBasicConsumer consumer = new(Channel);
        consumer.ReceivedAsync += Handler;
        await Channel.BasicConsumeAsync(Queue, autoAck: false, consumer: consumer, cancellationToken: ct);
    }

    public async Task Shutdown(CancellationToken ct = default)
    {
        await Channel.CloseAsync(ct);
    }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler => async (_, @event) =>
    {
        Logger.Information("Received message to add spares.");
        try
        {
            AddSparesMessage message = AddSparesMessage.CreateFromDeliverEventArgs(@event);
            AddSparesCommand command = CreateCommand(message);
            await using AsyncServiceScope scope = Services.CreateAsyncScope();
            ICommandHandler<AddSparesCommand, int> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<AddSparesCommand, int>>();
            await handler.Execute(command);
            await Channel.BasicAckAsync(@event.DeliveryTag, false);
        }
        catch(Exception e)
        {
            Logger.Fatal(e, "Failed to process message.");
            await Channel.BasicNackAsync(@event.DeliveryTag, false, false);
        }
    };

    private sealed class AddSparesMessage
    {
        public AddSpareMessagePayload[] Payload { get; set; } = [];
        
        public static AddSparesMessage CreateFromDeliverEventArgs(BasicDeliverEventArgs @event) => 
            JsonSerializer.Deserialize<AddSparesMessage>(@event.Body.Span)!;
    }

    private sealed class AddSpareMessagePayload
    {
        public Guid CreatorId { get; set; } = Guid.Empty;
        public string CreatorType { get; set; } = string.Empty;
        public string CreatorDomain { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public Guid ContainedItemId { get; set; } = Guid.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsNds { get; set; }
        public string Oem { get; set; } = string.Empty;
        public string[] Photos { get; set; } = [];
        public long Price { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    private static AddSparesCommand CreateCommand(AddSparesMessage message)
    {
        AddSpareCommandPayload[] spareInfos = message.Payload.Select(ConvertToAddSpareCommandCreatorInfo).ToArray();
        return new AddSparesCommand(Spares: spareInfos.ToArray());
    }
    
    private static AddSpareCommandPayload ConvertToAddSpareCommandCreatorInfo(AddSpareMessagePayload payload)
    {
        return new AddSpareCommandPayload(
            CreatorId: payload.CreatorId,
            CreatorDomain: payload.CreatorDomain,
            CreatorType: payload.CreatorType,
            SpareId: payload.Id,
            ContainedItemId: payload.ContainedItemId,
            Source: payload.Url,
            Title: payload.Title,
            Oem: payload.Oem,
            Price: payload.Price,
            IsNds: payload.IsNds,
            Address: payload.Address,
            Type: payload.Type,
            PhotoPaths: payload.Photos
            );
    }
}
using System.Text.Json;
using ContainedItems.Domain.Features.AddContainedItems;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ContainedItems.Infrastructure.Consumers;

public sealed class AddContainedItemsConsumer(RabbitMqConnectionSource rabbitMq, Serilog.ILogger logger, IServiceProvider serviceProvider) : IConsumer
{
    private const string Exchange = "contained-items";
    private const string Queue = "contained-items.create";
    private const string RoutingKey = "contained-items.create";
    
    private IChannel? _channel;
    private Serilog.ILogger Logger { get; } = logger.ForContext<AddContainedItemsConsumer>();
    private IServiceProvider Services { get; } = serviceProvider;
    private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
    private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel was not initialized");
    
    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
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
        await Channel.CloseAsync(cancellationToken: ct);
    }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler => async (_, @event) =>
    {
        Logger.Information("Handling message");
        try
        {
            AddContainedItemsMessage message = AddContainedItemsMessage.FromEventArgs(@event);
            IEnumerable<AddContainedItemsBody> commandPayload = message.Items.Select(i =>
            {
                return new AddContainedItemsBody(
                    i.ItemId, 
                    message.CreatorId, 
                    message.CreatorDomain,
                    message.CreatorType, i.Content);
            });
            
            await using AsyncServiceScope scope = Services.CreateAsyncScope();
            ICommandHandler<AddContainedItemsCommand, int> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<AddContainedItemsCommand, int>>();
            
            AddContainedItemsCommand command = new(commandPayload);
            Result<int> inserted = await handler.Execute(command);
            if (inserted.IsSuccess) Logger.Information("Added {Count} items.", inserted.Value);
            await Channel.BasicAckAsync(@event.DeliveryTag, false);
        }
        catch(Exception e)
        {
            Logger.Fatal(e, "Failed to process message.");
            await Channel.BasicNackAsync(@event.DeliveryTag, false, false);
        }
    };
    
    private sealed class AddContainedItemsMessage
    {
        public Guid CreatorId { get; set; }
        public string CreatorDomain { get; set; }
        public string CreatorType { get; set; }
        public AddContainedItemMessagePayload[] Items { get; set; }
        
        public static AddContainedItemsMessage FromEventArgs(BasicDeliverEventArgs ea)
        {
            return JsonSerializer.Deserialize<AddContainedItemsMessage>(ea.Body.Span)!;
        }
    }

    private sealed class AddContainedItemMessagePayload
    {
        public string ItemId { get; set; }
        public string Content { get; set; }
    }
}
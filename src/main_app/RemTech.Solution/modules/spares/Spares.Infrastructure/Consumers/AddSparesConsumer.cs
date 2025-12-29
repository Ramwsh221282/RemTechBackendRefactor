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
            AddSpareMessage message = AddSpareMessage.CreateFromDeliverEventArgs(@event);
            AddSparesCommand command = CreateCommand(message);
            await using AsyncServiceScope scope = Services.CreateAsyncScope();
            ICommandHandler<AddSparesCommand, int> handler =
                scope.ServiceProvider.GetRequiredService<ICommandHandler<AddSparesCommand, int>>();
            await handler.Execute(command);
        }
        catch(Exception e)
        {
            Logger.Fatal(e, "Failed to process message.");
            await Channel.BasicNackAsync(@event.DeliveryTag, false, false);
        }
    };

    private sealed class AddSpareMessage
    {
        public Guid CreatorId { get; set; } = Guid.Empty;
        public string CreatorType { get; set; } = string.Empty;
        public string CreatorDomain { get; set; } = string.Empty;
        public AddSpareMessagePayload[] Items { get; set; } = [];

        public static AddSpareMessage CreateFromDeliverEventArgs(BasicDeliverEventArgs @event) => 
            JsonSerializer.Deserialize<AddSpareMessage>(@event.Body.Span)!;
    }

    private sealed class AddSpareMessagePayload
    {
        public string id { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public Guid contained_item_id { get; set; } = Guid.Empty;
    }

    private static AddSparesCommand CreateCommand(AddSpareMessage message)
    {
        AddSpareCommandCreatorInfo creatorInfo = ConvertToAddSpareCommandCreatorInfo(message);
        IEnumerable<AddSpareCommandSpareInfo> spareInfos = message.Items.Select(ConvertToAddSpareCommandInfo);
        return new AddSparesCommand(Spares: spareInfos.ToArray(), Creator: creatorInfo);
    }
    
    private static AddSpareCommandCreatorInfo ConvertToAddSpareCommandCreatorInfo(
        AddSpareMessage message
        )
    {
        return new AddSpareCommandCreatorInfo(
            CreatorId: message.CreatorId,
            CreatorDomain: message.CreatorDomain,
            CreatorType: message.CreatorType);
    }

    private static AddSpareCommandSpareInfo ConvertToAddSpareCommandInfo(AddSpareMessagePayload payload)
    {
        string id = payload.id;
        Guid containedItemId = payload.contained_item_id;
        using JsonDocument document = JsonDocument.Parse(payload.content);
        string address = document.RootElement.GetProperty("address").GetString()!;
        long price = document.RootElement.GetProperty("price").GetInt64();
        bool is_nds = document.RootElement.GetProperty("is_nds").GetBoolean();
        string oem = document.RootElement.GetProperty("oem").GetString()!;
        string[] photos = document.RootElement.GetProperty("photos").EnumerateArray().Select(p => p.GetString()!).ToArray();
        string url = document.RootElement.GetProperty("url").GetString()!;
        string title = document.RootElement.GetProperty("title").GetString()!;
        string type = document.RootElement.GetProperty("type").GetString()!;
        return new AddSpareCommandSpareInfo(
            SpareId: id,
            ContainedItemId: containedItemId,
            Source: url,
            Title: title,
            Oem: oem,
            Price: price,
            IsNds: is_nds,
            Address: address,
            Type: type,
            PhotoPaths: photos);
    }
}
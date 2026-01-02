using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Vehicles.Domain.Features.AddVehicle;

namespace Vehicles.Infrastructure.RabbitMq.Consumers;

public sealed class AddVehicleConsumer(
    IServiceProvider services, 
    Serilog.ILogger logger, 
    RabbitMqConnectionSource rabbitMq) : IConsumer
{
    private const string Exchange = "vehicles";
    private const string Queue = "vehicles.add";
    private const string RoutingKey = "vehicles.add";

    private IChannel? _channel;
    private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel was not initialized");
    private IServiceProvider Services { get; } = services;
    private Serilog.ILogger Logger { get; } = logger.ForContext<AddVehicleConsumer>();
    private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;

    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
    {
        _channel = await TopicConsumerInitialization.InitializeChannel(RabbitMq, Exchange, Queue, RoutingKey, ct);
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        AsyncEventingBasicConsumer consumer = new(Channel);
        consumer.ReceivedAsync += Handler;
        await Channel.BasicConsumeAsync(Queue, false, consumer, ct);
    }

    public async Task Shutdown(CancellationToken ct = default)
    {
        await Channel.CloseAsync(ct);
    }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler => async (_, @event) =>
    {
        Logger.Information("Handling message.");
        try
        {
            AddVehicleMessage message = AddVehicleMessage.CreateFrom(@event);
            if (!IsMessageValid(message, out string error))
            {
                Logger.Error("Denied message: {Error}", error);
                await Channel.BasicAckAsync(@event.DeliveryTag, false);
                return;
            }

            AddVehicleCommand command = CreateCommandFrom(message);
            int saved = await SaveVehicles(Services, command);
            Logger.Information("Vehicles added. Saved {Saved}", saved);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error handling message.");
        }
        finally
        {
            await Channel.BasicAckAsync(@event.DeliveryTag, false);
        }
    };

    private static async Task<Result<int>> SaveVehicles(IServiceProvider services, AddVehicleCommand command)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        return await scope.ServiceProvider
            .GetRequiredService<ICommandHandler<AddVehicleCommand, int>>()
            .Execute(command);
    }
    
    private static AddVehicleCommand CreateCommandFrom(AddVehicleMessage message)
    {
        AddVehicleCreatorCommandPayload creator = CreateCreatorPayload(message);
        IEnumerable<AddVehicleVehiclesCommandPayload> vehicles = CreateVehiclesPayload(message);
        return new AddVehicleCommand(creator, vehicles);
    }

    private static AddVehicleCreatorCommandPayload CreateCreatorPayload(AddVehicleMessage message) => 
        new(CreatorId: message.CreatorId, 
            CreatorDomain: message.CreatorDomain, 
            CreatorType: message.CreatorType);

    private static IEnumerable<AddVehicleVehiclesCommandPayload> CreateVehiclesPayload(AddVehicleMessage message)
    {
        return message.Payload.Select(p => new AddVehicleVehiclesCommandPayload(
            Id: p.Id,
            Title: p.Title,
            Url: p.Url,
            Price: p.Price,
            IsNds: p.IsNds,
            Address: p.Address,
            Photos: p.Photos,
            Characteristics: p.Characteristics.Select(c => new AddVehicleCommandCharacteristics(c.Name, c.Value)).ToArray()
            ));
    }
    
    private static bool IsMessageValid(AddVehicleMessage message, out string error)
    {
        List<string> errors = [];
        if (message.CreatorId == Guid.Empty) errors.Add("Идентификатор создателя пуст");
        if (string.IsNullOrWhiteSpace(message.CreatorType)) errors.Add("Тип создателя пуст");
        if (string.IsNullOrWhiteSpace(message.CreatorDomain)) errors.Add("Домен создателя пуст");
        if (message.Payload == null || !message.Payload.Any()) errors.Add("Список автомобилей пуст");
        error = string.Join(", ", errors);
        return errors.Count == 0;
    }
    
    private sealed class AddVehicleMessage
    {
        public required Guid CreatorId { get; set; }
        public required string CreatorDomain { get; set; }
        public required string CreatorType { get; set; }
        public required IEnumerable<AddVehicleMessagePayload> Payload { get; set; }

        public static AddVehicleMessage CreateFrom(BasicDeliverEventArgs @event)
        {
            return JsonSerializer.Deserialize<AddVehicleMessage>(@event.Body.Span)!;
        }
    }
    
    private sealed class AddVehicleMessagePayload
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Url { get; set; }
        public required long Price { get; set; }
        public required bool IsNds { get; set; }
        public required string Address { get; set; }
        public required string[] Photos { get; set; }
        public required IEnumerable<AddVehicleCharacteristic> Characteristics { get; set; }
    }
    
    private sealed class AddVehicleCharacteristic
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
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
            AddVehicleMessagePayload payload = AddVehicleMessagePayload.FromDeliverEventArgs(@event);
            AddVehicleCommand command = new(
                CreatorId: payload.CreatorId,
                CreatorDomain: payload.CreatorDomain,
                CreatorType: payload.CreatorType,
                Id: payload.Id,
                Title: payload.Title,
                Url: payload.Url,
                Price: payload.Price,
                IsNds: payload.IsNds,
                Address: payload.Address,
                Photos: payload.Photos,
                Characteristics: payload.Characteristics.Select(c => new AddVehicleCommandCharacteristics(c.Name, c.Value)).ToArray()
            );

            await using AsyncServiceScope scope = Services.CreateAsyncScope();
            ICommandHandler<AddVehicleCommand, Unit> handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<AddVehicleCommand, Unit>>();
            Result<Unit> result = await handler.Execute(command);
            if (result.IsFailure)
            {
                logger.Error("Failed to add vehicle. {Error}", result.Error.Message);
            }
            else
            {
                logger.Information("Vehicle {Id} added.", payload.Id);
            }
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
    
    private sealed class AddVehicleMessagePayload
    {
        public required Guid CreatorId { get; set; } = Guid.Empty;
        public required string CreatorDomain { get; set; } = string.Empty;
        public required string CreatorType { get; set; } = string.Empty;
        public required Guid Id { get; set; } = Guid.Empty;
        public required string Title { get; set; } = string.Empty;
        public required string Url { get; set; } = string.Empty;
        public required long Price { get; set; }
        public required bool IsNds { get; set; }
        public required string Address { get; set; } = string.Empty;
        public required string[] Photos { get; set; } = [];
        public required AddVehicleCharacteristic[] Characteristics { get; set; } = [];

        public static AddVehicleMessagePayload FromDeliverEventArgs(BasicDeliverEventArgs @event)
        {
            return JsonSerializer.Deserialize<AddVehicleMessagePayload>(@event.Body.Span)!;
        }
    }
    
    private sealed class AddVehicleCharacteristic
    {
        public required string Name { get; set; } = string.Empty;
        public required string Value { get; set; } = string.Empty;
    }
}
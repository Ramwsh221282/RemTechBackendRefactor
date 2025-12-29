using System.Text;
using System.Text.Json;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.BackgroundServices;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ContainedItems.Infrastructure.Producers;

public sealed class AddVehiclesProducer(RabbitMqProducer producer, Serilog.ILogger logger) : IItemPublishingStrategy
{
    private const string Exchange = "vehicles";
    private const string RoutingKey = "vehicles.add";
    private RabbitMqProducer Producer { get; } = producer;
    private Serilog.ILogger Logger { get; } = logger.ForContext<AddVehiclesProducer>();
    
    public async Task Publish(ContainedItem item, CancellationToken ct = default)
    {
        Logger.Information("Publishing {Id}", item.Id);
        ReadOnlyMemory<byte> payload = CreatePayload(item);
        RabbitMqPublishOptions options = new() { Persistent = true };
        await Producer.PublishDirectAsync(payload, Exchange, RoutingKey, options, ct: ct);
        Logger.Information("Published {Id}", item.Id);
    }

    private ReadOnlyMemory<byte> CreatePayload(ContainedItem item)
    {
        using JsonDocument document = JsonDocument.Parse(item.Info.Content);
        
        List<AddVehicleCharacteristic> characteristics = [];
        foreach (var ctx in document.RootElement.GetProperty("characteristics").EnumerateArray())
        {
            string name = ctx.GetProperty("name").GetString()!;
            string value = ctx.GetProperty("value").GetString()!;
            characteristics.Add(new AddVehicleCharacteristic { Name = name, Value = value });
        }
        
        
        AddVehicleMessagePayload payload = new()
        {
            CreatorId = item.CreatorInfo.CreatorId,
            CreatorDomain = item.CreatorInfo.Domain,
            CreatorType = item.CreatorInfo.Type,
            Id = item.ServiceItemId.Value,
            Title = document.RootElement.GetProperty("title").GetString()!,
            Characteristics = characteristics.ToArray(),
            Url = document.RootElement.GetProperty("url").GetString()!,
            Price = document.RootElement.GetProperty("price").GetInt64(),
            IsNds = document.RootElement.GetProperty("is_nds").GetBoolean(),
            Address = document.RootElement.GetProperty("address").GetString()!,
            Photos = document.RootElement.GetProperty("photos").EnumerateArray().Select(p => p.GetString()!).ToArray(),
        };
        
        string json = JsonSerializer.Serialize(payload);
        return Encoding.UTF8.GetBytes(json);
    }
    
    private sealed class AddVehicleMessagePayload
    {
        public required Guid CreatorId { get; set; }
        public required string CreatorDomain { get; set; }
        public required string CreatorType { get; set; }
        public required string Id { get; set; }
        public required string Title { get; set; }
        public required string Url { get; set; }
        public required long Price { get; set; }
        public required bool IsNds { get; set; }
        public required string Address { get; set; }
        public required string[] Photos { get; set; }
        public required AddVehicleCharacteristic[] Characteristics { get; set; }
    }
    
    private sealed class AddVehicleCharacteristic
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
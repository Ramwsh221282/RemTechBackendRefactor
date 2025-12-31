using System.Text.Json;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.BackgroundServices;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ContainedItems.Infrastructure.Producers;

public sealed class AddSparesProducer(RabbitMqProducer producer, Serilog.ILogger logger) : IItemPublishingStrategy
{
    private const string Exchange = "spares";
    private const string RoutingKey = "spares.add";
    private RabbitMqProducer Producer { get; } = producer;
    private Serilog.ILogger Logger { get; } = logger.ForContext<AddSparesProducer>();
    
    public async Task Publish(ContainedItem[] items, CancellationToken ct = default)
    {
        Logger.Information("Publishing {Count}", items.Length);
        if (items.Length == 0)
        {
            Logger.Information("No items to publish");
            return;
        }
        
        AddSparesMessage message = new() { Payload = CreatePayload(items) };
        RabbitMqPublishOptions options = new() { Persistent = true };
        await Producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct: ct);
        Logger.Information("Published {Count}", items.Length);
    }
    
    private static AddSpareMessagePayload[] CreatePayload(ContainedItem[] item)
    {
        return item.Select(i =>
        {
            using JsonDocument document = JsonDocument.Parse(i.Info.Content);
            return new AddSpareMessagePayload()
            {
                CreatorId = i.CreatorInfo.CreatorId,
                CreatorDomain = i.CreatorInfo.Domain,
                CreatorType = i.CreatorInfo.Type,
                Address = document.RootElement.GetProperty("address").GetString()!,
                ContainedItemId = i.Id.Value,
                Id = i.ServiceItemId.Value,
                IsNds = document.RootElement.GetProperty("is_nds").GetBoolean(),
                Oem = document.RootElement.GetProperty("oem").GetString()!,
                Photos = document.RootElement.GetProperty("photos").EnumerateArray().Select(p => p.GetString()!).ToArray(),
                Price = document.RootElement.GetProperty("price").GetInt64(),
                Title = document.RootElement.GetProperty("title").GetString()!,
                Type = document.RootElement.GetProperty("type").GetString()!,
                Url = document.RootElement.GetProperty("url").GetString()!,
            };
        }).ToArray();
    }
    
    private sealed class AddSparesMessage
    {
        public AddSpareMessagePayload[] Payload { get; set; } = [];
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

    public async Task Publish(ContainedItem item, CancellationToken ct = default)
    {
        await Publish([item], ct);
    }
}
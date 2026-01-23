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
		AddVehicleMessagePayload message = CreatePayload(item);
		RabbitMqPublishOptions options = new() { Persistent = true };
		await Producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct: ct);
		Logger.Information("Published {Id}", item.Id);
	}

	public async Task PublishMany(IEnumerable<ContainedItem> items, CancellationToken ct = default)
	{
		ContainedItem[] itemArray = [.. items];
		if (itemArray.Length == 0)
		{
			Logger.Information("No items to publish");
			return;
		}

		RabbitMqPublishOptions options = new() { Persistent = true };
		foreach (IGrouping<Guid, ContainedItem> group in itemArray.GroupBy(i => i.CreatorInfo.CreatorId))
		{
			ContainedItem first = group.First();
			AddVehicleMessage message = CreateMessage(first, group);
			await Producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct: ct);
			Logger.Information(
				"Published {CreatorType} {CreatorDomain}",
				first.CreatorInfo.Type,
				first.CreatorInfo.Domain
			);
		}
	}

	private AddVehicleMessage CreateMessage(ContainedItem first, IEnumerable<ContainedItem> items)
	{
		Guid creatorId = first.CreatorInfo.CreatorId;
		string creatorType = first.CreatorInfo.Type;
		string creatorDomain = first.CreatorInfo.Domain;
		return new()
		{
			CreatorId = creatorId,
			CreatorDomain = creatorDomain,
			CreatorType = creatorType,
			Payload = items.Select(CreatePayload),
		};
	}

	private AddVehicleMessagePayload CreatePayload(ContainedItem item)
	{
		using JsonDocument document = JsonDocument.Parse(item.Info.Content);

		List<AddVehicleCharacteristic> characteristics = [];
		foreach (JsonElement ctx in document.RootElement.GetProperty("characteristics").EnumerateArray())
		{
			string name = ctx.GetProperty("name").GetString()!;
			string value = ctx.GetProperty("value").GetString()!;
			characteristics.Add(new AddVehicleCharacteristic { Name = name, Value = value });
		}

		AddVehicleMessagePayload payload = new()
		{
			Id = item.Id.Value,
			Title = document.RootElement.GetProperty("title").GetString()!,
			Characteristics = characteristics,
			Url = document.RootElement.GetProperty("url").GetString()!,
			Price = document.RootElement.GetProperty("price").GetInt64(),
			IsNds = document.RootElement.GetProperty("is_nds").GetBoolean(),
			Address = document.RootElement.GetProperty("address").GetString()!,
			Photos = document.RootElement.GetProperty("photos").EnumerateArray().Select(p => p.GetString()!).ToArray(),
		};

		return payload;
	}

	private sealed class AddVehicleMessage
	{
		public required Guid CreatorId { get; set; }
		public required string CreatorDomain { get; set; }
		public required string CreatorType { get; set; }
		public required IEnumerable<AddVehicleMessagePayload> Payload { get; set; }
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

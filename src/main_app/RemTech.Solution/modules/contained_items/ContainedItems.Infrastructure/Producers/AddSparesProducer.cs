using System.Text.Json;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.BackgroundServices;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ContainedItems.Infrastructure.Producers;

/// <summary>
/// Производитель для добавления запчастей.
/// </summary>
/// <param name="producer">Производитель RabbitMQ для отправки сообщений.</param>
/// <param name="logger">Логгер для записи информации и ошибок.</param>
public sealed class AddSparesProducer(RabbitMqProducer producer, Serilog.ILogger logger) : IItemPublishingStrategy
{
	private const string Exchange = "spares";
	private const string RoutingKey = "spares.add";
	private RabbitMqProducer Producer { get; } = producer;
	private Serilog.ILogger Logger { get; } = logger.ForContext<AddSparesProducer>();

	/// <summary>
	/// Публикует содержащиеся элементы.
	/// </summary>
	/// <param name="items">Список содержащихся элементов для публикации.</param>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	public async Task Publish(IEnumerable<ContainedItem> items, CancellationToken ct = default)
	{
		ContainedItem[] itemArray = [.. items];
		if (itemArray.Length == 0)
		{
			Logger.Information("No items to publish");
			return;
		}

		RabbitMqPublishOptions options = new() { Persistent = true };
		foreach (IGrouping<Guid, ContainedItem> entry in itemArray.GroupBy(i => i.CreatorInfo.CreatorId))
		{
			ContainedItem first = entry.First();
			AddSparesMessage message = CreateMessage(first, entry);
			await Producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct: ct);
			Logger.Information(
				"Published {CreatorType} {CreatorDomain}",
				first.CreatorInfo.Type,
				first.CreatorInfo.Domain
			);
		}
	}

	/// <summary>
	/// Публикует содержащийся элемент.
	/// </summary>
	/// <param name="item">Содержащийся элемент для публикации.</param>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	public Task Publish(ContainedItem item, CancellationToken ct = default) => Publish([item], ct);

	/// <summary>
	/// Публикует множество содержащихся элементов.
	/// </summary>
	/// <param name="items">Список содержащихся элементов для публикации.</param>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	public Task PublishMany(IEnumerable<ContainedItem> items, CancellationToken ct = default) => Publish(items, ct);

	private static AddSparesMessage CreateMessage(ContainedItem first, IEnumerable<ContainedItem> items) =>
		new()
		{
			CreatorId = first.CreatorInfo.CreatorId,
			CreatorDomain = first.CreatorInfo.Type,
			CreatorType = first.CreatorInfo.Domain,
			Payload = CreatePayload(items),
		};

	private static IEnumerable<AddSpareMessagePayload> CreatePayload(IEnumerable<ContainedItem> item) =>
		item.Select(i =>
		{
			using JsonDocument document = JsonDocument.Parse(i.Info.Content);
			return new AddSpareMessagePayload()
			{
				Address = document.RootElement.GetProperty("address").GetString()!,
				ContainedItemId = i.Id.Value,
				IsNds = document.RootElement.GetProperty("is_nds").GetBoolean(),
				Oem = document.RootElement.GetProperty("oem").GetString()!,
				Photos = document
					.RootElement.GetProperty("photos")
					.EnumerateArray()
					.Select(p => p.GetString()!)
					.ToArray(),
				Price = document.RootElement.GetProperty("price").GetInt64(),
				Title = document.RootElement.GetProperty("title").GetString()!,
				Type = document.RootElement.GetProperty("type").GetString()!,
				Url = document.RootElement.GetProperty("url").GetString()!,
			};
		});

	private sealed class AddSparesMessage
	{
		public Guid CreatorId { get; set; } = Guid.Empty;
		public string CreatorType { get; set; } = string.Empty;
		public string CreatorDomain { get; set; } = string.Empty;
		public IEnumerable<AddSpareMessagePayload> Payload { get; set; } = [];
	}

	private sealed class AddSpareMessagePayload
	{
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
}

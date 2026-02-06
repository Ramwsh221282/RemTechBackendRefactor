namespace ParsingSDK.RabbitMq;

public sealed class AddContainedItemsMessagePayload
{
    public required string ItemId { get; set; } = string.Empty;
    public required string Content { get; set; } = string.Empty;
}
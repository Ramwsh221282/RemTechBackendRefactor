namespace ParsingSDK.RabbitMq;



public sealed class AddContainedItemsMessage
{
    public required Guid CreatorId { get; set; } = Guid.Empty;
    public required string CreatorType { get; set; } = string.Empty;
    public required string CreatorDomain { get; set; } = string.Empty;
    public required string ItemType { get; set; } = string.Empty;
    public required AddContainedItemsMessagePayload[] Items { get; set; } = [];
}
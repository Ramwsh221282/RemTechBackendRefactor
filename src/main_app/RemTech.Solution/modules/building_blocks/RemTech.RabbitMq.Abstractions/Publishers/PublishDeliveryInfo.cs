namespace RemTech.RabbitMq.Abstractions.Publishers;

public sealed record PublishDeliveryInfo(string CorellationId, string MessageId);
namespace RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers;

public sealed record PublishDeliveryInfo(string CorellationId, string MessageId);
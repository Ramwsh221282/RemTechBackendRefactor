namespace RemTech.RabbitMq.Abstractions.Publishers;

public interface IRabbitMqPublisher<TOptions> 
    where TOptions : IRabbitMqPublishOptions
{
    Task<PublishDeliveryInfo> Publish(TOptions options, CancellationToken ct = default);
}
namespace RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers;

public interface IRabbitMqPublisher<in TOptions> 
    where TOptions : IRabbitMqPublishOptions
{
    Task<PublishDeliveryInfo> Publish(TOptions options, CancellationToken ct = default);
}

public interface IRabbitMqPublisher
{
    Task<PublishDeliveryInfo> Publish(string message, CancellationToken ct = default);
}
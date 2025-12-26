using RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers;

namespace Identity.Gateways.Common;

public interface IRabbitMqOutboxMessagePublisher : IRabbitMqPublisher
{
    public string SupportedMessageType { get; }
}
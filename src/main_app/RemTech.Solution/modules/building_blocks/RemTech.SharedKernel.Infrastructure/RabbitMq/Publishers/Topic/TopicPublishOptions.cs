namespace RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers.Topic;

public sealed record TopicPublishOptions(
    string Message, 
    string Queue, 
    string Exchange, 
    string RoutingKey) : IRabbitMqPublishOptions;
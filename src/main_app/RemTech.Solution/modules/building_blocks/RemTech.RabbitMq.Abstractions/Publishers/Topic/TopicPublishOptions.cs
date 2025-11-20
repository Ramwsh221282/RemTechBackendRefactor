namespace RemTech.RabbitMq.Abstractions.Publishers.Topic;

public sealed record TopicPublishOptions(
    string Message, 
    string Queue, 
    string Exchange, 
    string RoutingKey) : IRabbitMqPublishOptions;
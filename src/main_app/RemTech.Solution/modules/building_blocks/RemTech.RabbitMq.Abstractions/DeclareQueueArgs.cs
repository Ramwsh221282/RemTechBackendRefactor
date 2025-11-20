namespace RemTech.RabbitMq.Abstractions;

public sealed record DeclareQueueArgs(
    string QueueName, 
    string Exchange, 
    string RoutingKey, 
    string ExchangeType);
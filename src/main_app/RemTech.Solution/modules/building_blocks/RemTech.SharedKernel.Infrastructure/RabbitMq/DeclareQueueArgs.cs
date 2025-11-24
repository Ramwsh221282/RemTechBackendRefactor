namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed record DeclareQueueArgs(
    string QueueName, 
    string Exchange, 
    string RoutingKey, 
    string ExchangeType);
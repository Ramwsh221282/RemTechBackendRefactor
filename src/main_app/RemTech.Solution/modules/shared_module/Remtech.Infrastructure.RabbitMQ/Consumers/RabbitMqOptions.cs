namespace Remtech.Infrastructure.RabbitMQ.Consumers;

public sealed class RabbitMqOptions
{
    public required string Hostname { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Port { get; set; }
}

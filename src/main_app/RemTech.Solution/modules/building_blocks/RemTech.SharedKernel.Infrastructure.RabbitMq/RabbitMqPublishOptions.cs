namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed class RabbitMqPublishOptions
{
    public bool Persistent { get; set; } = true;
    public bool Mandatory { get; set; } = true;
}

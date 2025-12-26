namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface IMessagePublisher<in TMessage> where TMessage : Message
{
    Task Publish(TMessage message, CancellationToken ct = default);
}
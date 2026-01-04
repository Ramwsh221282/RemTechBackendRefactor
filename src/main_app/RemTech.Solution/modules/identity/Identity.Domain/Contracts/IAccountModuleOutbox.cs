namespace Identity.Domain.Contracts;

public interface IAccountModuleOutbox
{
    Task Add<TMessage>(OutboxMessage<TMessage> message, CancellationToken ct = default) where TMessage : IOutboxMessagePayload;
}
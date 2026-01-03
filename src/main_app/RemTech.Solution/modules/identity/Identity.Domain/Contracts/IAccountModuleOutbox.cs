namespace Identity.Domain.Contracts;

public interface IAccountModuleOutbox
{
    Task Add<TOutboxMessage>(TOutboxMessage message, CancellationToken ct = default) where TOutboxMessage : IAccountOutboxMessage;
}
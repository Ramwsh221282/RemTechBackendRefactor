namespace Identity.Domain.Contracts.Outbox;

public interface IAccountModuleOutbox
{
    Task Add(IdentityOutboxMessage message, CancellationToken ct = default);
    Task<IdentityOutboxMessage[]> GetMany(OutboxMessageSpecification spec, CancellationToken ct = default);
}
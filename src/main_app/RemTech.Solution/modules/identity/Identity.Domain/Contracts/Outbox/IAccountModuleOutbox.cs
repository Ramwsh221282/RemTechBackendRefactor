namespace Identity.Domain.Contracts.Outbox;

public interface IAccountModuleOutbox
{
    public Task Add(IdentityOutboxMessage message, CancellationToken ct = default);
    public Task<IdentityOutboxMessage[]> GetMany(OutboxMessageSpecification spec, CancellationToken ct = default);
}

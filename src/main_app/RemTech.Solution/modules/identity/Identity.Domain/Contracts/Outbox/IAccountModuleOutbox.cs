namespace Identity.Domain.Contracts.Outbox;

public interface IAccountModuleOutbox
{
    Task Add(OutboxMessage message, CancellationToken ct = default);
    Task<OutboxMessage[]> GetMany(OutboxMessageSpecification spec, CancellationToken ct = default);
}
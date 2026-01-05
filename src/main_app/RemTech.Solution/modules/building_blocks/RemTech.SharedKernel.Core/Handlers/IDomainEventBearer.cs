namespace RemTech.SharedKernel.Core.Handlers;

public interface IDomainEventBearer
{
    IReadOnlyList<IDomainEvent> Events { get; }
}
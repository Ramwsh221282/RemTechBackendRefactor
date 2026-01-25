namespace Spares.Domain.Features;

public interface IOnSparesAddedEventPublisher
{
    public Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default);
}

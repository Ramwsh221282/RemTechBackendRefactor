namespace Spares.Domain.Features;

public interface IOnSparesAddedEventPublisher
{
	Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default);
}

using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Spares.Domain.Features;

public sealed class AddSparesEventTransporter(IOnSparesAddedEventPublisher publisher)
	: IEventTransporter<AddSparesCommand, (Guid, int)>
{
	public async Task Transport((Guid, int) result, CancellationToken ct = default)
	{
		if (result.Item1 == Guid.Empty)
			return;
		if (result.Item2 == 0)
			return;
		await publisher.Publish(result.Item1, result.Item2, ct);
	}
}

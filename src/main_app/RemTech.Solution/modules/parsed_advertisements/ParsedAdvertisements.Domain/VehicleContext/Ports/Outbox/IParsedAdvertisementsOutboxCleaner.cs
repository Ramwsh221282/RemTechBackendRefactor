using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;

public interface IParsedAdvertisementsOutboxCleaner
{
    Task Remove(
        ITransactionManager transaction,
        CancellationToken ct = default
    );
}
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;

public interface IParsedAdvertisementsOutboxDeliverer
{
    Task<Status> Save(
        ParsedAdvertisementsOutboxMessage message,
        ITransactionManager transaction,
        CancellationToken ct = default
    );
}
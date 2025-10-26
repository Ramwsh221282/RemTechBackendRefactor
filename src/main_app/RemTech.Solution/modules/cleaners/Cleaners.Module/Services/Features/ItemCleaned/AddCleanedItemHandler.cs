using Cleaners.Module.Contracts.ItemCleaned;
using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.ItemCleaned;

internal sealed class AddCleanedItemHandler(
    ICleaners cleaners,
    Serilog.ILogger logger,
    ItemCleanedMessagePublisher publisher
) : ICommandHandler<AddCleanedItem>
{
    public async Task Handle(AddCleanedItem command, CancellationToken ct = default)
    {
        logger.Information("Adding cleaned item");
        ICleaner cleaner = await cleaners.Single(ct);
        ICleaner withCleanedItem = cleaner.CleanItem();
        ICleaner logged = await withCleanedItem
            .ProduceOutput()
            .PrintTo(new LoggingCleanerVeil(logger))
            .BehaveAsync(ct);
    }
}

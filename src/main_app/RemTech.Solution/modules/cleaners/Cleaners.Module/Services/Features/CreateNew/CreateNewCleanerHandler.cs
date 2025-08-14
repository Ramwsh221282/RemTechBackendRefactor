using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.CreateNew;

internal sealed class CreateNewCleanerHandler(Serilog.ILogger logger)
    : ICommandHandler<CreateNewCleaner, ICleaner>
{
    public Task<ICleaner> Handle(CreateNewCleaner command, CancellationToken ct = default)
    {
        logger.Information("Creating new cleaner");
        ICleaner cleaner = Cleaner.Create(
            Guid.NewGuid(),
            0,
            DateTime.UtcNow,
            DateTime.UtcNow,
            1,
            "Ожидает",
            0,
            0,
            0,
            1
        );
        return Task.FromResult(cleaner);
    }
}

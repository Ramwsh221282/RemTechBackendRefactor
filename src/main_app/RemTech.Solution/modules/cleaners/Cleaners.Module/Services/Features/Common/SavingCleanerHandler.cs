using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.Common;

internal sealed class SavingCleanerHandler<TCommand>(
    NpgsqlConnection connection,
    ICommandHandler<TCommand, ICleaner> inner
) : ICommandHandler<TCommand, ICleaner>
    where TCommand : ICommand
{
    public async Task<ICleaner> Handle(TCommand command, CancellationToken ct = default)
    {
        ICleaner fromOrigin = await inner.Handle(command, ct);
        fromOrigin = await fromOrigin
            .ProduceOutput()
            .PrintTo(new NpgSqlSavingCleanerVeil(connection))
            .BehaveAsync(ct);
        return fromOrigin;
    }
}

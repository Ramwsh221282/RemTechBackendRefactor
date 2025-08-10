using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal interface ISpares<TPersistance>
{
    Task<bool> Persist(
        ISparePersistanceCommand<TPersistance> command,
        CancellationToken ct = default
    );
}

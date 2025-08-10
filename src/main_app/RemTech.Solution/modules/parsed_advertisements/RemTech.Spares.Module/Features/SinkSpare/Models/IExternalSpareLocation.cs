namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal interface IExternalSpareLocation
{
    Task<SpareLocation> Fetch(CancellationToken ct = default);
}

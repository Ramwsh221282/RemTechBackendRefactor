namespace Cleaners.Module.Domain;

internal interface ICleaners
{
    Task<ICleaner> Single(CancellationToken ct = default);
}

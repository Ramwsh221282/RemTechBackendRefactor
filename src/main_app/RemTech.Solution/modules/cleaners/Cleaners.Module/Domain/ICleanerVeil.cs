namespace Cleaners.Module.Domain;

internal interface ICleanerVeil
{
    void Accept(
        Guid id,
        int cleanedAmount,
        DateTime lastRun,
        DateTime nextRun,
        int waitDays,
        string state,
        int hours,
        int minutes,
        int seconds
    );

    ICleaner Behave();

    Task<ICleaner> BehaveAsync(CancellationToken ct = default);
}

namespace Cleaners.Module.Endpoints.Responses;

internal sealed record CleanerHttpResponseOutput(
    Guid Id,
    int CleanedAmount,
    DateTime LastRun,
    DateTime NextRun,
    int WaitDays,
    string State,
    int Hours,
    int Minutes,
    int Seconds,
    int ItemsThreshold
);

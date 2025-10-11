namespace Telemetry.Contracts;

/// <summary>
/// Контракт добавления действия пользователя в БД.
/// </summary>
public sealed record SaveActionInfoEvent(
    IEnumerable<string> Comments,
    string Name,
    string Status,
    Guid InvokerId,
    DateTime? OccuredAt
);

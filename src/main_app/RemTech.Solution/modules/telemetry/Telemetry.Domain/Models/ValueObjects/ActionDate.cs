using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.Models.ValueObjects;

/// <summary>
/// Дата записи действия телеметрии
/// </summary>
public readonly record struct ActionDate
{
    public DateTime OccuredAt { get; }

    public ActionDate() => OccuredAt = DateTime.UtcNow;

    private ActionDate(DateTime occuredAt) => OccuredAt = occuredAt;

    public static Status<ActionDate> Create(DateTime? occuredAt) =>
        occuredAt == null ? new ActionDate() : Create(occuredAt.Value);

    public static Status<ActionDate> Create(DateTime occuredAt) =>
        occuredAt switch
        {
            _ when occuredAt == DateTime.MaxValue => Error.Validation(
                "Дата записи телеметрии некорректна."
            ),
            _ when occuredAt == DateTime.MinValue => Error.Validation(
                "Дата записи телеметрии некорректна."
            ),
            _ => new ActionDate(occuredAt),
        };
}

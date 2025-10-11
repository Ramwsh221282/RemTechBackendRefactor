using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

/// <summary>
/// Комментарий к действию
/// </summary>
public sealed record TelemetryActionComment
{
    public string Value { get; }

    private TelemetryActionComment(string value) => Value = value;

    public static Status<TelemetryActionComment> Create(string? value) =>
        value switch
        {
            null => Error.Validation(
                "Комментарий к записи действия телеметрии не может быть пустым."
            ),
            not null when string.IsNullOrWhiteSpace(value) => Error.Validation(
                "Комментарий к записи действия телеметрии не может быть пустым."
            ),
            _ => new TelemetryActionComment(value),
        };
}

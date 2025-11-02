using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Primitives;

public readonly record struct NotEmptyDateTime
{
    public DateTime Value { get; }

    private NotEmptyDateTime(DateTime value) => Value = value;

    public NotEmptyDateTime() => Value = DateTime.Now;

    public static Status<NotEmptyDateTime> Create(DateTime value)
    {
        if (value == DateTime.MinValue || value == DateTime.MaxValue)
            return Error.Validation("Дата некорректная");
        return value;
    }
}
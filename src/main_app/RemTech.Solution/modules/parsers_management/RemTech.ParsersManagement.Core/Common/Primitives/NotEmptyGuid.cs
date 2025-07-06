using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Primitives;

public sealed class NotEmptyGuid
{
    private readonly Guid _value;

    private NotEmptyGuid(Guid value) => _value = value;

    public bool Same(NotEmptyGuid other) => _value == other._value;

    public static NotEmptyGuid New() => new(Guid.NewGuid());

    public static Status<NotEmptyGuid> New(Guid? value) =>
        value switch
        {
            null => new Error("ID было пустым.", ErrorCodes.Validation),
            not null when value.Value == Guid.Empty => new Error(
                "ID было пустым.",
                ErrorCodes.Validation
            ),
            _ => new NotEmptyGuid(value.Value),
        };

    public static implicit operator Guid(NotEmptyGuid id) => id._value;

    public static implicit operator string(NotEmptyGuid id) => id._value.ToString();

    public override bool Equals(object? obj) =>
        obj switch
        {
            NotEmptyGuid other => other._value == _value,
            _ => false,
        };

    public override int GetHashCode() => _value.GetHashCode();
}

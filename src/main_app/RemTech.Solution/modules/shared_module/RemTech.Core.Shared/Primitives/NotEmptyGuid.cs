using RemTech.Result.Library;

namespace RemTech.Core.Shared.Primitives;

public readonly record struct NotEmptyGuid
{
    private readonly Guid _value = Guid.NewGuid();

    public NotEmptyGuid(Guid? value) => _value = value ?? Guid.Empty;

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

    public static implicit operator bool(NotEmptyGuid nguid)
    {
        return nguid._value != Guid.Empty;
    }

    public static implicit operator bool(NotEmptyGuid? nguid)
    {
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();
}

public readonly record struct NewGuid
{
    private readonly NotEmptyGuid _value;

    public NewGuid() => _value = new NotEmptyGuid(Guid.NewGuid());

    public static implicit operator Guid(NewGuid guid) => guid._value;

    public static implicit operator NotEmptyGuid(NewGuid guid) => guid._value;
}

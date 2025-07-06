using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Primitives;

public sealed class NotEmptyStringLength
{
    private readonly int _length;

    public NotEmptyStringLength(NotEmptyString nes)
    {
        string nesString = nes;
        _length = string.IsNullOrWhiteSpace(nesString) ? 0 : nesString.Length;
    }

    public static implicit operator int(NotEmptyStringLength length) => length._length;

    public static implicit operator string(NotEmptyStringLength length) =>
        length._length.ToString();

    public override bool Equals(object? obj) =>
        obj switch
        {
            NotEmptyStringLength l => l._length == _length,
            _ => false,
        };

    public override int GetHashCode() => _length.GetHashCode();
}

public sealed class NotEmptyString
{
    private readonly string _value;

    public NotEmptyString(string? value) =>
        _value = string.IsNullOrWhiteSpace(value) ? string.Empty : value;

    public string StringValue() => _value;

    public bool Same(NotEmptyString other) => other._value == _value;

    public override string ToString() => _value;

    public static Status<NotEmptyString> New(string? input) =>
        string.IsNullOrWhiteSpace(input)
            ? new Error("Строка не должна быть пустой.", ErrorCodes.Validation)
            : new NotEmptyString(input);

    public static implicit operator string(NotEmptyString notEmptyString) => notEmptyString._value;

    public static implicit operator bool(NotEmptyString notEmptyString) =>
        !string.IsNullOrWhiteSpace(notEmptyString._value);

    public override bool Equals(object? obj) =>
        obj switch
        {
            NotEmptyString other => other._value == _value,
            _ => false,
        };

    public override int GetHashCode() => _value.GetHashCode();
}

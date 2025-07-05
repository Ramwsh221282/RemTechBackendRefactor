using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Primitives;

public sealed class NotEmptyString
{
    private readonly string _value;

    private NotEmptyString(string value) => _value = value;

    public string StringValue() => _value;

    public bool Same(NotEmptyString other) => other._value == _value;

    public int Length() => _value.Length;

    public override string ToString() => _value;

    public static Status<NotEmptyString> New(string? input) =>
        string.IsNullOrWhiteSpace(input)
            ? new Error("Строка не должна быть пустой.", ErrorCodes.Validation)
            : new NotEmptyString(input);

    public static implicit operator string(NotEmptyString notEmptyString) => notEmptyString._value;
}

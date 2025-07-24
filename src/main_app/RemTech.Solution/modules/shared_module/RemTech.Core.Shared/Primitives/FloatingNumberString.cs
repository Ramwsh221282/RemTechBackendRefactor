using System.Text.RegularExpressions;

namespace RemTech.Core.Shared.Primitives;

public sealed partial class FloatingNumberString
{
    private const RegexOptions Options =
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    [GeneratedRegex(@"(\d+(?:[.,]\d+)?)", Options)]
    private static partial Regex FloatingNumberRegex();

    private readonly NotEmptyString _value;
    private readonly bool _matched;

    public FloatingNumberString(NotEmptyString value)
    {
        Match match = FloatingNumberRegex().Match(value);
        _matched = match.Success;
        _value = match.Success ? new NotEmptyString(match.Groups[1].Value) : value;
    }

    public static implicit operator bool(FloatingNumberString floatingNumberString)
    {
        return floatingNumberString._value && floatingNumberString._matched;
    }

    public string Read() => _value;
}
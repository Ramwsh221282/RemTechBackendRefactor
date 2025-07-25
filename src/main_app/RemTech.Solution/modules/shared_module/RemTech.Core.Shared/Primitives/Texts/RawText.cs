namespace RemTech.Core.Shared.Primitives.Texts;

public sealed class RawText : IText
{
    private readonly string? _value;

    public RawText(string? value)
    {
        _value = value;
    }

    public string Read()
    {
        return string.IsNullOrEmpty(_value) ? string.Empty : _value;
    }
}

namespace RemTech.Core.Shared.Primitives.Texts;

public sealed class TrimmedText : IText
{
    private readonly IText _text;

    public TrimmedText(IText text)
    {
        _text = text;
    }

    public string Read()
    {
        string text = _text.Read();
        return !string.IsNullOrEmpty(text) ? text.Trim() : text;
    }
}

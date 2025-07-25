namespace RemTech.Core.Shared.Primitives.Texts;

public sealed class CapitalizedFirstLetterText : IText
{
    private readonly IText _text;

    public CapitalizedFirstLetterText(IText text)
    {
        _text = text;
    }

    public string Read()
    {
        string text = _text.Read();
        return !string.IsNullOrEmpty(text)
            ? string.Concat(text[0].ToString().ToUpper(), text.AsSpan(1))
            : text;
    }
}

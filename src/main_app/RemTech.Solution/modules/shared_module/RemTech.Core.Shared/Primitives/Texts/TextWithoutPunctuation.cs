namespace RemTech.Core.Shared.Primitives.Texts;

public sealed class TextWithoutPunctuation : IText
{
    private readonly IText _text;

    public TextWithoutPunctuation(IText text)
    {
        _text = text;
    }

    public string Read()
    {
        string text = _text.Read();
        if (text.Contains(','))
            text = text.Replace(",", "");
        if (text.Contains('.'))
            text = text.Replace(".", "");
        if (text.Contains('!'))
            text = text.Replace("!", "");
        if (text.Contains('?'))
            text = text.Replace("?", "");
        return text;
    }
}

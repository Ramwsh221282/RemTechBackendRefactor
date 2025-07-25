namespace RemTech.Core.Shared.Primitives.Texts;

public sealed class TextWithoutDigits : IText
{
    private readonly IText _text;

    public TextWithoutDigits(IText text)
    {
        _text = text;
    }

    public string Read()
    {
        string unformatted = _text.Read();
        string formatted = new(unformatted.Where(c => char.IsDigit(c) == false).ToArray());
        return formatted;
    }
}

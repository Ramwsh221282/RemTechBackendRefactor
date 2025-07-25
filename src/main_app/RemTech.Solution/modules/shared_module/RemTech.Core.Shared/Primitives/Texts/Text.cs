namespace RemTech.Core.Shared.Primitives.Texts;

public sealed class Text : IText
{
    private readonly IText _text;

    public Text(IText text) => _text = text;

    public string Read()
    {
        return _text.Read();
    }
}

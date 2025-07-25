using System.Text.RegularExpressions;

namespace RemTech.Core.Shared.Primitives.Texts;

public sealed partial class TextWithOnlyRussianLetters : IText
{
    private readonly IText _origin;

    public TextWithOnlyRussianLetters(IText origin)
    {
        _origin = origin;
    }

    public string Read()
    {
        string _text = _origin.Read();
        string withoutEnglish = EnglishLettersRegex().Replace(_text, " ");
        return withoutEnglish;
    }

    [GeneratedRegex("[a-zA-Z]", RegexOptions.Compiled)]
    private static partial Regex EnglishLettersRegex();
}

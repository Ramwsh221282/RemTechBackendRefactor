using System.Text.RegularExpressions;

namespace RemTech.Core.Shared.Primitives.Texts;

public sealed partial class TextWithOnlyOneWhiteSpaces : IText
{
    private readonly IText _origin;

    public TextWithOnlyOneWhiteSpaces(IText origin)
    {
        _origin = origin;
    }

    public string Read()
    {
        string _text = this._origin.Read();
        return MultipleWhitespacesRegex().Replace(_text, " ");
    }

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex MultipleWhitespacesRegex();
}
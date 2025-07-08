using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common;

public sealed record ParsedItemText
{
    private readonly NotEmptyString _text;

    public ParsedItemText(NotEmptyString text)
    {
        _text = text;
    }

    public ParsedItemText(string? text)
        : this(new NotEmptyString(text)) { }

    public static implicit operator NotEmptyString(ParsedItemText text)
    {
        return text._text;
    }

    public static implicit operator string(ParsedItemText text)
    {
        return text._text;
    }

    public static implicit operator bool(ParsedItemText text)
    {
        return text._text;
    }
}

namespace RemTech.Core.Shared.Primitives;

public sealed class ShortDateString(DateTime date)
{
    private string _text = string.Empty;

    public string Read() =>
        !string.IsNullOrEmpty(_text) ? _text : _text = $"{date.Day} / {date.Month} / {date.Year}";
}

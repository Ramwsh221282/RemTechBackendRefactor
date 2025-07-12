namespace RemTech.ParsedAdvertisements.DataSource.Adapter;

public sealed class TsQuerySequenceSensitiveString
{
    private readonly string _unformatted;

    public TsQuerySequenceSensitiveString(string unformatted)
    {
        _unformatted = unformatted;
    }

    public string AsTsQueryString()
    {
        string[] parts = _unformatted.Split(' ', StringSplitOptions.TrimEntries);
        return string.Join("<->", parts);
    }
}

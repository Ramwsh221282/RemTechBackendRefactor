namespace RemTech.ParsedAdvertisements.DataSource.Adapter;

public sealed class TsQueryMaybeSimilarString
{
    private readonly string _unformatted;

    public TsQueryMaybeSimilarString(string unformatted)
    {
        _unformatted = unformatted;
    }

    public string AsTsQueryString()
    {
        string[] parts = _unformatted.Split(' ', StringSplitOptions.TrimEntries);
        return string.Join("|", parts);
    }
}
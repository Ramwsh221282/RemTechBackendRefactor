namespace RemTech.ParsedAdvertisements.DataSource.Adapter;

public sealed class WhereFilterSqlString
{
    private readonly List<string> _terms = [];

    public WhereFilterSqlString With(string term)
    {
        _terms.Add(term);
        return this;
    }

    public WhereFilterSqlString WithIf<T>(string term, T value, Func<T, bool> predicate)
    {
        return predicate(value) ? With(term) : this;
    }

    public int Amount() => _terms.Count;

    public string AsSqlString()
    {
        return string.Join(" AND ", _terms);
    }
}

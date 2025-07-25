namespace Parsing.SDK.ElementSources;

public sealed class ElementPathMode
{
    private readonly string _path;

    public ElementPathMode(string path) => _path = path;

    public string Mode()
    {
        if (string.IsNullOrWhiteSpace(_path))
            return "EMPTY PATH.";
        if (_path.StartsWith('.'))
            return "CLASS NAME.";
        if (_path.StartsWith('#'))
            return "ID";
        return "CSS SELECTOR.";   
    }
}
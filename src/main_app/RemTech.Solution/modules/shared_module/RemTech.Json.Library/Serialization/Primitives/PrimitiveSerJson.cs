namespace RemTech.Json.Library.Serialization.Primitives;

public abstract class PrimitiveSerJson : ISerJson
{
    private readonly string _key;
    private readonly string _artifact;

    public PrimitiveSerJson(string key, bool value)
    {
        _key = key;
        _artifact = $"\"{key}\":{value.ToString().ToLower()}";
    }

    public PrimitiveSerJson(string key, DateTime value)
    {
        _key = key;
        _artifact = $"\"{key}\":\"{value:o}\"";
    }

    public PrimitiveSerJson(string key, Guid value)
    {
        _key = key;
        _artifact = $"\"{key}\":\"{value}\"";
    }

    public PrimitiveSerJson(string key, int value)
    {
        _key = key;
        _artifact = $"\"{key}\":{value}";
    }

    public PrimitiveSerJson(string key, long value)
    {
        _key = key;
        _artifact = $"\"{key}\":{value}";
    }

    public PrimitiveSerJson(string key, double value)
    {
        _key = key;
        _artifact = $"\"{key}\":{value}";
    }

    public PrimitiveSerJson(string key, string value)
    {
        _key = key;
        _artifact = $"\"{key}\":\"{value}\"";
    }

    public string Read()
    {
        return _artifact;
    }

    public string Key()
    {
        return _key;
    }
}

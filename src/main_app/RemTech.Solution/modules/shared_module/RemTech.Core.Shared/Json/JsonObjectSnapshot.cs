using System.Text.Json;

namespace RemTech.Core.Shared.Json;

public sealed class JsonObjectSnapshot
{
    private readonly Dictionary<string, object> _data = [];

    public JsonObjectSnapshot With(string key, object value)
    {
        _data.Add(key, value);
        return this;
    }

    public string Read()
    {
        return JsonSerializer.Serialize(_data);
    }
}

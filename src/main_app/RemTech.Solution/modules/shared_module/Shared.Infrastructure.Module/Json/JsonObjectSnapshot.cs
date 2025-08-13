using System.Text.Json;

namespace Shared.Infrastructure.Module.Json;

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

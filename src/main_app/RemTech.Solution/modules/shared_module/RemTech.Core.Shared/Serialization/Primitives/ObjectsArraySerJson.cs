namespace RemTech.Core.Shared.Serialization.Primitives;

public sealed class ObjectsArraySerJson<T> : ISerJson
{
    private readonly string _key;
    private readonly IEnumerable<T> _items;
    private readonly Queue<PlainSerJson> _artifacts = [];

    public ObjectsArraySerJson(string key, IEnumerable<T> items)
    {
        _key = key;
        _items = items;
    }

    public ObjectsArraySerJson<T> ForEach(Func<T, PlainSerJson> convertFn)
    {
        foreach (T item in _items)
            _artifacts.Enqueue(convertFn.Invoke(item));
        return this;
    }

    public string Read()
    {
        Queue<string> jsons = [];
        while (_artifacts.Count > 0)
            jsons.Enqueue(_artifacts.Dequeue().Read());
        return $"\"{_key}\":[{string.Join(',', jsons)}]";
    }

    public string Key()
    {
        return _key;
    }
}

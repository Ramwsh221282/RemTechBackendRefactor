using System.Reflection;

namespace RemTech.Core.Shared.Reflection;

public sealed class FieldsSearcher(object @from)
{
    private readonly FieldInfo[] _fields = @from.GetType().GetFields();
    private readonly Dictionary<string, object> _values = [];

    public T SearchByName<T>(string name)
    {
        if (_values.TryGetValue(name, out var value))
            return (T)value;
        object searched = _fields.Single(f => f.Name == name).GetValue(@from)!;
        T result = (T)searched;
        _values.TryAdd(name, searched);
        return result;
    }
}
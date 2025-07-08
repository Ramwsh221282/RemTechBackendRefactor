using System.Text.Json;

namespace RemTech.Json.Library.Deserialization.Primitives;

public sealed class DesJsonArray : IDisposable
{
    private readonly JsonElement.ArrayEnumerator _enumerator;
    private readonly int _length;

    public DesJsonArray(JsonElement element)
    {
        _length = element.GetArrayLength();
        _enumerator = element.EnumerateArray();
    }

    public void Dispose() => _enumerator.Dispose();

    public T[] MapEach<T>(Func<DesJsonArrayElement, T> map)
    {
        T[] array = new T[_length];
        int idx = 0;
        foreach (JsonElement element in _enumerator)
        {
            DesJsonArrayElement arrayElement = new(element);
            array[idx] = map(arrayElement);
            idx++;
        }
        return array;
    }
}

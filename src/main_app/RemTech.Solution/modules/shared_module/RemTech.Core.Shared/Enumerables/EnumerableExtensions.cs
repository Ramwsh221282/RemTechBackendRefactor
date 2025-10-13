using System.Diagnostics.CodeAnalysis;

namespace RemTech.Core.Shared.Enumerables;

public static class EnumerableExtensions
{
    public static bool HasRepeatableValues<T, TKey>(
        this IEnumerable<T> enumerable,
        Func<T, TKey> selector,
        out T[] repeatableValues
    )
    {
        T[] array = enumerable.ToArray();
        T[] distinct = array.DistinctBy(selector).ToArray();

        if (array.Length == distinct.Length)
        {
            repeatableValues = [];
            return false;
        }

        repeatableValues = array.GetRepeatableValues(selector);
        return true;
    }

    public static bool MatchTo<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate,
        [NotNullWhen(true)] out T? matched
    )
    {
        foreach (T item in source)
        {
            if (!predicate(item))
                continue;

            matched = item;
            return true;
        }

        matched = default;
        return false;
    }

    public static bool MatchTo<T, TValue>(
        this IEnumerable<T> source,
        Func<T, TValue> factory,
        Func<TValue, bool> predicate,
        [NotNullWhen(true)] out TValue matched
    )
    {
        foreach (T item in source)
        {
            TValue value = factory(item);
            if (predicate(value))
            {
                matched = value;
                return true;
            }
        }

        matched = default!;
        return false;
    }

    public static string CreateRepeatableValuesMessage<T>(
        this IEnumerable<T> source,
        Func<T, string> selector,
        string prefix
    )
    {
        IEnumerable<string> values = source.Select(selector);
        return $"{prefix} {string.Join(" ,", values)}";
    }

    public static T[] GetRepeatableValues<T, TKey>(
        this IEnumerable<T> enumerable,
        Func<T, TKey> selector
    ) => enumerable.GroupBy(selector).Where(g => g.Count() > 1).SelectMany(g => g).ToArray();
}

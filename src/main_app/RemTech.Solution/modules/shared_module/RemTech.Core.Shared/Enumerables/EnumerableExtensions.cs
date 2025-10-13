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

    public static T[] GetRepeatableValues<T, TKey>(
        this IEnumerable<T> enumerable,
        Func<T, TKey> selector
    ) => enumerable.GroupBy(selector).Where(g => g.Count() > 1).SelectMany(g => g).ToArray();
}

namespace RemTech.Core.Shared.Enumerable;

public static class EnumerableExtensions
{
    public static bool AllUnique<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> selector)
    {
        var array = enumerable.ToArray();
        var distinct = array.DistinctBy(selector).ToArray();
        return array.Length == distinct.Length;
    }
}
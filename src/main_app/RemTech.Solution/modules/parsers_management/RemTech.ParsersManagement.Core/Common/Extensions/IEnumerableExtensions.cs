using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Extensions;

public static class IEnumerableExtensions
{
    public static MaybeBag<T> Maybe<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        MaybeBag<T> bag = new();
        T? value = enumerable.FirstOrDefault(predicate);
        return value == null ? bag : bag.Put(value);
    }
}

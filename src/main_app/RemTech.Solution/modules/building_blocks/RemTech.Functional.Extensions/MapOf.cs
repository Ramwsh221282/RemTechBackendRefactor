namespace RemTech.Functional.Extensions;

public class MapOf
{
    public static MapOf<T, U> Map<T, U>(Func<T, U> mapFunction)
    {
        return new MapOf<T, U>(mapFunction);
    }
}

public sealed class MapOf<T, U>
{
    private readonly Func<T, U> _mapFunction;

    internal MapOf(Func<T, U> mapFunction)
    {
        _mapFunction = mapFunction;
    }

    internal static MapOf<T, U> Map(Func<T, U> mapFunction) => new(mapFunction);

    public U Map(T from) => _mapFunction(from);
    public U this[T key] => _mapFunction(key);
}
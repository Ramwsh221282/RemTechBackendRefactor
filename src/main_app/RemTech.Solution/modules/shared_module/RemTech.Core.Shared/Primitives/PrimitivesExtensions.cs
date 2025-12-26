namespace RemTech.Core.Shared.Primitives;

public static class PrimitivesExtensions
{
    public static string Set(this string source, Func<string> other)
    {
        source = other();
        return source;
    }

    public static Guid Set(this Guid source, Func<Guid> other)
    {
        source = other();
        return source;
    }

    public static int Set(this int source, Func<int> other)
    {
        source = other();
        return source;
    }

    public static T CloneTo<T>(this T @object, Func<T, T> other)
    {
        return other(@object);
    }
}
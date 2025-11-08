namespace RemTech.Primitives.Extensions;

public static class Strings
{
    public static bool EmptyOrWhiteSpace(string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static bool NotEmptyOrWhiteSpace(string? str)
    {
        bool em = EmptyOrWhiteSpace(str);
        bool notEm = !em;
        return notEm;
    }

    public static bool GreaterThan(string str, int length)
    {
        return str.Length > length;
    }

    public static bool NotGreaterThan(string str, int length)
    {
        bool greater = GreaterThan(str, length);
        bool notGreater = !greater;
        return notGreater;
    }

    public static bool LesserThan(string str, int length)
    {
        return str.Length < length;
    }

    public static bool NotLesserThan(string str, int length)
    {
        return !LesserThan(str, length);
    }
}
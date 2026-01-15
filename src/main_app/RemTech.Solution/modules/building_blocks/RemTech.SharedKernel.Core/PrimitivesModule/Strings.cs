namespace RemTech.SharedKernel.Core.PrimitivesModule;

public static class Strings
{
    extension(string[] source)
    {
        public string Join(char separator)
        {
            return Joined(source, separator);
        }

        public string Join(string separator)
        {
            return Joined(source, separator);
        }
    }

    public static string Joined(string[] source, char separator)
    {
        return string.Join(separator, source);
    }

    public static string Joined(List<string> source, char separator)
    {
        return string.Join(separator, source);
    }

    public static string Joined(IEnumerable<string> source, char separator)
    {
        return string.Join(separator, source);
    }

    public static string Joined(List<string> source, string separator)
    {
        return string.Join(separator, source);
    }

    public static string Joined(IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source);
    }

    public static string Joined(string[] source, string separator)
    {
        return string.Join(separator, source);
    }

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

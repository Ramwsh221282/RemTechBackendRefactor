namespace RemTech.SharedKernel.Core.PrimitivesModule;

public static class Strings
{
    extension(string[] source)
    {
        public string Join(char separator) => Joined(source, separator);

        public string Join(string separator) => Joined(source, separator);
    }

    public static string Joined(string[] source, char separator) => string.Join(separator, source);

    public static string Joined(IEnumerable<string> source, char separator) => string.Join(separator, source);

    public static string Joined(IEnumerable<string> source, string separator) => string.Join(separator, source);

    public static string Joined(string[] source, string separator) => string.Join(separator, source);

    public static bool EmptyOrWhiteSpace(string? str) => string.IsNullOrWhiteSpace(str);

    public static bool NotEmptyOrWhiteSpace(string? str)
    {
        bool empty = EmptyOrWhiteSpace(str);
        return !empty;
    }

    public static bool GreaterThan(string str, int length) => str.Length > length;

    public static bool NotGreaterThan(string str, int length)
    {
        bool greater = GreaterThan(str, length);
        return !greater;
    }

    public static bool LesserThan(string str, int length) => str.Length < length;

    public static bool NotLesserThan(string str, int length) => !LesserThan(str, length);
}

namespace RemTech.Core.Shared.Extensions;

public static class CommonExtensions
{
    public static Task<T> FromResult<T>(this T source)
    {
        return Task.FromResult(source);
    }

    public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);
}

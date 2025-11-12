namespace RemTech.Primitives.Extensions;

public static class Guids
{
    public static bool NotEmpty(Guid? id)
    {
        return id.HasValue;
    }

    public static bool NotEmpty(Guid id)
    {
        return id != Guid.Empty;
    }
}
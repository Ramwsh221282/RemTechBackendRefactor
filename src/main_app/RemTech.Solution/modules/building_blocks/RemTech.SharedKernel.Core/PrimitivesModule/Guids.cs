namespace RemTech.SharedKernel.Core.PrimitivesModule;

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

    public static bool Empty(Guid id)
    {
        return id == Guid.Empty;
    }

    public static bool Empty(Guid? id)
    {
        if (id.HasValue)
        {
            return id.Value == Guid.Empty;
        }

        return true;
    }
}

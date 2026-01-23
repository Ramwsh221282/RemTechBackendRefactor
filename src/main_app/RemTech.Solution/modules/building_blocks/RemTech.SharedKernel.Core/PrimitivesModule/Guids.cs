namespace RemTech.SharedKernel.Core.PrimitivesModule;

public static class Guids
{
    public static bool NotEmpty(Guid? id) => id.HasValue;

    public static bool NotEmpty(Guid id) => id != Guid.Empty;

    public static bool Empty(Guid id) => id == Guid.Empty;

    public static bool Empty(Guid? id)
    {
        return id.HasValue ? id.Value == Guid.Empty : true;
    }
}

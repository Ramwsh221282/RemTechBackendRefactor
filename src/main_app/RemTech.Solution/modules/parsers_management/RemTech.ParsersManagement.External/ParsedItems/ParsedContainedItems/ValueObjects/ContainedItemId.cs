using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsersManagement.External.ParsedItems.ParsedContainedItems.ValueObjects;

public sealed record ContainedItemId
{
    private readonly NotEmptyString _id;

    public ContainedItemId(NotEmptyString id)
    {
        _id = id;
    }

    public ContainedItemId(string? id)
        : this(new NotEmptyString(id)) { }

    public static implicit operator string(ContainedItemId id)
    {
        return id._id;
    }

    public static implicit operator NotEmptyString(ContainedItemId id)
    {
        return id._id;
    }

    public static implicit operator bool(ContainedItemId id)
    {
        return id._id;
    }
}

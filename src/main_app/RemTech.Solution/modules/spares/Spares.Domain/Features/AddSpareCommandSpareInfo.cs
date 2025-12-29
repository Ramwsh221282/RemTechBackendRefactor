namespace Spares.Domain.Features;

public sealed record AddSpareCommandSpareInfo(
    string SpareId,
    Guid ContainedItemId,
    string Source,
    string Oem,
    string Title,
    long Price,
    bool IsNds,
    string Address,
    string Type,
    IEnumerable<string> PhotoPaths
);
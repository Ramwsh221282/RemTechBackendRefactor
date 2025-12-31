namespace Spares.Domain.Features;

public sealed record AddSpareCommandPayload(
    Guid CreatorId,
    string CreatorDomain,
    string CreatorType,
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
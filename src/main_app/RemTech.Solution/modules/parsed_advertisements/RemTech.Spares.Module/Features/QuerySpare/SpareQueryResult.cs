namespace RemTech.Spares.Module.Features.QuerySpare;

public sealed record SpareQueryResult(
    string Id,
    string City,
    Guid CityId,
    bool IsNds,
    string Title,
    IEnumerable<string> Photos,
    string Region,
    Guid RegionId,
    string SourceUrl,
    long PriceValue,
    string Description,
    string RegionKind
);

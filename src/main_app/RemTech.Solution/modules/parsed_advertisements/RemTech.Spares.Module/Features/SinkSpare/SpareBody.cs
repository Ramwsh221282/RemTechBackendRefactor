namespace RemTech.Spares.Module.Features.SinkSpare;

internal sealed record SpareBody(
    string Id,
    string Description,
    string Title,
    string SourceUrl,
    long PriceValue,
    bool IsNds,
    string LocationText,
    IEnumerable<string> Photos
);

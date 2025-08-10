namespace RemTech.Spares.Module.Features.SinkSpare;

internal sealed record Sparebody(
    string Id,
    string Description,
    string Title,
    string SourceUrl,
    long Value,
    bool IsNds,
    string LocationText,
    IEnumerable<string> Photos
);

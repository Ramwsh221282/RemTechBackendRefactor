namespace Parsing.RabbitMq.PublishSpare;

public sealed record SpareBody(
    string Id,
    string Description,
    string Title,
    string SourceUrl,
    long PriceValue,
    bool IsNds,
    string LocationText,
    IEnumerable<string> Photos
);

using Parsing.RabbitMq.PublishSpare;

namespace Avito.Parsing.Spares.Parsing;

public sealed class SpareBodyValidator
{
    public bool IsValid(SpareBody body)
    {
        return !string.IsNullOrWhiteSpace(body.Id)
            && !string.IsNullOrWhiteSpace(body.Description)
            && !string.IsNullOrWhiteSpace(body.Title)
            && !string.IsNullOrWhiteSpace(body.SourceUrl)
            && !string.IsNullOrWhiteSpace(body.LocationText);
    }
}

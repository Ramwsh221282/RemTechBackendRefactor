using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed record VehicleSourceSpecification
{
    public string Url { get; }
    public string Domain { get; }

    private VehicleSourceSpecification(string url, string domain)
    {
        Url = url;
        Domain = domain;
    }

    public static Status<VehicleSourceSpecification> Create(string sourceUrl, string sourceDomain)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
            return Error.Validation("Источник (ссылка) техники не может быть пустой.");

        if (string.IsNullOrWhiteSpace(sourceDomain))
            return Error.Validation("Домен техники не может быть пустым.");

        return new VehicleSourceSpecification(sourceUrl, sourceDomain);
    }
}

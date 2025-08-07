namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

public sealed class ParsedVehicleUrl
{
    private readonly string _url;

    public ParsedVehicleUrl(string? url)
    {
        _url = url ?? string.Empty;
    }

    public static implicit operator string(ParsedVehicleUrl url) => url._url;

    public static implicit operator bool(ParsedVehicleUrl url) =>
        !string.IsNullOrWhiteSpace(url._url);
}

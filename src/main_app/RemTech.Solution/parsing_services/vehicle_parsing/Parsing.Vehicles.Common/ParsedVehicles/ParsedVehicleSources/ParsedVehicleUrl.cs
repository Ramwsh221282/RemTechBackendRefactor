using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

public sealed class ParsedVehicleUrl
{
    private readonly NotEmptyString _url;

    public ParsedVehicleUrl(string? url) : this(new NotEmptyString(url))
    { }
    
    public ParsedVehicleUrl(NotEmptyString url) =>
        _url = url;

    public static implicit operator NotEmptyString(ParsedVehicleUrl url) 
        => url._url;

    public static implicit operator string(ParsedVehicleUrl url) =>
        url._url;

    public static implicit operator bool(ParsedVehicleUrl url) =>
        url._url;
}
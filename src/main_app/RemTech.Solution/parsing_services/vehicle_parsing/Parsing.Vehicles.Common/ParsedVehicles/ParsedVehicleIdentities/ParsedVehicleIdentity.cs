namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;

public sealed class ParsedVehicleIdentity(string? id)
{
    private readonly string _id = id ?? string.Empty;

    public static implicit operator string(ParsedVehicleIdentity identity) => identity._id;

    public static implicit operator bool(ParsedVehicleIdentity identity) =>
        !string.IsNullOrWhiteSpace(identity._id);
}

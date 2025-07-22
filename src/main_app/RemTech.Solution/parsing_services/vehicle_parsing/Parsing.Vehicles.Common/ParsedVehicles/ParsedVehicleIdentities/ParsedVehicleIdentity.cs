using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;

public sealed class ParsedVehicleIdentity
{
    private readonly NotEmptyString _id;
    public ParsedVehicleIdentity(NotEmptyString id) => _id = id;
    public ParsedVehicleIdentity(string? id) : this(new NotEmptyString(id)) { }
    public static implicit operator string(ParsedVehicleIdentity identity) => identity._id;
    public static implicit operator NotEmptyString(ParsedVehicleIdentity identity) => identity._id;
    public static implicit operator bool(ParsedVehicleIdentity identity) => identity._id;
}
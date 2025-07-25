using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

public sealed class ParsedVehicleKind
{
    private readonly NotEmptyString _kind;

    public ParsedVehicleKind(NotEmptyString kind) =>
        _kind = kind;

    public ParsedVehicleKind(string? kind) : this(new  NotEmptyString(kind))
    { }

    public static implicit operator NotEmptyString(ParsedVehicleKind kind) => kind._kind;
    public static implicit operator string(ParsedVehicleKind kind) => kind._kind;
    public static implicit operator bool(ParsedVehicleKind kind) => kind._kind;
}
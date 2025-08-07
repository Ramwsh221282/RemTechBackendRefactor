namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

public sealed class ParsedVehicleKind(string? kind)
{
    private readonly string _kind = kind ?? string.Empty;

    public static implicit operator string(ParsedVehicleKind kind) => kind._kind;

    public static implicit operator bool(ParsedVehicleKind kind) =>
        !string.IsNullOrWhiteSpace(kind._kind);
}

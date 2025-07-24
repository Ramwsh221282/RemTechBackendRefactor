namespace Parsing.Vehicles.Common.Json;

public sealed class ParsedVehicleBytes
{
    private readonly byte[] _bytes;

    public ParsedVehicleBytes(byte[] bytes)
    {
        _bytes = bytes;
    }

    public byte[] Read() => _bytes;
}
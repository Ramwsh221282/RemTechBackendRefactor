using System.Text;

namespace Parsing.Vehicles.Common.Json;

public sealed class ParsedVehicleJson
{
    private readonly string _json;

    public ParsedVehicleJson(string json)
    {
        _json = json;
    }

    public ParsedVehicleBytes Bytes()
    {
        return new ParsedVehicleBytes(Encoding.UTF8.GetBytes(_json));
    }

    public string Read() => _json;
}
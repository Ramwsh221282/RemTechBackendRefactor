namespace Parsing.Vehicles.Common.Json;

public sealed class ParsedVehicleParser
{
    private readonly string _parserName;
    private readonly string _parserType;
    private readonly string _linkName;

    public ParsedVehicleParser(string parserName, string parserType, string linkName)
    {
        _parserName = parserName;
        _parserType = parserType;
        _linkName = linkName;
    }

    public string ParserName() => _parserName;
    public string ParserType() => _parserType;
    public string LinkName() => _linkName;
}
namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class NoMeasurement : IMeasurement
{
    public string Read()
    {
        return string.Empty;
    }
}
namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class KgsMeasurement : IMeasurement
{
    public string Read()
    {
        return "кг";
    }
}
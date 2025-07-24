namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class MillimetersMeasurement : IMeasurement
{
    public string Read()
    {
        return "мм";
    }
}
namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class YearMeasurement : IMeasurement
{
    public string Read()
    {
        return "год";
    }
}
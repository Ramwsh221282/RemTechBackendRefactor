namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class NewtonMetresMeasurement : IMeasurement
{
    public string Read()
    {
        return "нм";
    }
}
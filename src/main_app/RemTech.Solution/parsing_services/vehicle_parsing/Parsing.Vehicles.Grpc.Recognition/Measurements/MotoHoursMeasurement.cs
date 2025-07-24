namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class MotoHoursMeasurement : IMeasurement
{
    public string Read()
    {
        return "моточасов";
    }
}
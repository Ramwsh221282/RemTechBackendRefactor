namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class LitresMeasurement : IMeasurement
{
    public string Read()
    {
        return "л";
    }
}
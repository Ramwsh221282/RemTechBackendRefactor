namespace Parsing.Vehicles.Grpc.Recognition.Measurements;

public sealed class BucketControlTypeMeasurement : IMeasurement
{
    public string Read()
    {
        return "управление";
    }
}
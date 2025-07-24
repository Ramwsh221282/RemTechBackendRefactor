using Parsing.Vehicles.Grpc.Recognition.Measurements;

namespace Parsing.Vehicles.Grpc.Recognition.BucketControlType;

public sealed class MeasurementBucketControlTypeRecognition : IBucketControlTypeRecognition
{
    private readonly IBucketControlTypeRecognition _origin;

    public MeasurementBucketControlTypeRecognition(IBucketControlTypeRecognition origin)
    {
        _origin = origin;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic ctx = await _origin.Recognize(text);
        return !ctx ? ctx : new Characteristic(ctx, new BucketControlTypeMeasurement());
    }
}
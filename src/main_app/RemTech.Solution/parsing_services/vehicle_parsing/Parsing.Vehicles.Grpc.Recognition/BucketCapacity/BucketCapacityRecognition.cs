namespace Parsing.Vehicles.Grpc.Recognition.BucketCapacity;

public sealed class BucketCapacityRecognition : IBucketCapacityRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("BUCKET_CAPACITY");
    private readonly string _ctxName = string.Intern("Объём ковша");

    public BucketCapacityRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}
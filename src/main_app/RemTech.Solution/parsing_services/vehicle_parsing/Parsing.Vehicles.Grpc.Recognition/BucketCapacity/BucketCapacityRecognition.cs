namespace Parsing.Vehicles.Grpc.Recognition.BucketCapacity;

public sealed class BucketCapacityRecognition(ICommunicationChannel channel)
    : IBucketCapacityRecognition
{
    private readonly string _ctxKey = string.Intern("BUCKET_CAPACITY");
    private readonly string _ctxName = string.Intern("Объём ковша");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

namespace Parsing.Vehicles.Grpc.Recognition.BucketControlType;

public sealed class BucketControlTypeRecognition : IBucketControlTypeRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("BUCKET_CONTROL_TYPE");
    private readonly string _ctxName = string.Intern("Тип управления копательным ковшом");

    public BucketControlTypeRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}
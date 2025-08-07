namespace Parsing.Vehicles.Grpc.Recognition.BucketControlType;

public sealed class BucketControlTypeRecognition(ICommunicationChannel channel)
    : IBucketControlTypeRecognition
{
    private readonly string _ctxKey = string.Intern("BUCKET_CONTROL_TYPE");
    private readonly string _ctxName = string.Intern("Тип управления копательным ковшом");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

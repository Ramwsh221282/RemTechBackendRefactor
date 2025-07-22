namespace Parsing.Vehicles.Grpc.Recognition.Vin;

public sealed class VinRecognition : IVinRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("VIN");

    public VinRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxKey);
    }
}
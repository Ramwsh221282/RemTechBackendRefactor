namespace Parsing.Vehicles.Grpc.Recognition.LoadingHeight;

public sealed class LoadingHeightRecognition : ILoadingHeightRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("LOADING_HEIGHT");
    private readonly string _ctxName = string.Intern("Высота подъёма");

    public LoadingHeightRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}
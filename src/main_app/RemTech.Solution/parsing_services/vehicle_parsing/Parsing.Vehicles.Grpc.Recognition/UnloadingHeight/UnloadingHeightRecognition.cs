namespace Parsing.Vehicles.Grpc.Recognition.UnloadingHeight;

public sealed class UnloadingHeightRecognition : IUnloadingHeightRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("UNLOADING_HEIGHT");
    private readonly string _ctxName = string.Intern("Высота выгрузки");

    public UnloadingHeightRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}
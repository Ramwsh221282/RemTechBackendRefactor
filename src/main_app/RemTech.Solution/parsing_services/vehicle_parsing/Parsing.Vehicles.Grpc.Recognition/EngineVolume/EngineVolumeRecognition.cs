namespace Parsing.Vehicles.Grpc.Recognition.EngineVolume;

public sealed class EngineVolumeRecognition : IEngineVolumeRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("ENGINE_VOLUME");
    private readonly string _ctxName = string.Intern("Объём двигателя");

    public EngineVolumeRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}
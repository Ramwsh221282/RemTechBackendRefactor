namespace Parsing.Vehicles.Grpc.Recognition.EngineType;

public sealed class EngineTypeRecognition : IEngineTypeRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("ENGINE_TYPE");
    private readonly string _ctxName = string.Intern("Тип двигателя");

    public EngineTypeRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}
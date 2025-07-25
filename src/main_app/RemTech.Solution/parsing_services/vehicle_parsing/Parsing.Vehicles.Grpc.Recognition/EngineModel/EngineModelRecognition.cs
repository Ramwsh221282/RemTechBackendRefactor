namespace Parsing.Vehicles.Grpc.Recognition.EngineModel;

public sealed class EngineModelRecognition : IEngineModelRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("ENGINE_MODEL");
    private readonly string _ctxName = string.Intern("Двигатель");

    public EngineModelRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}
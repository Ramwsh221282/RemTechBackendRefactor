namespace Parsing.Vehicles.Grpc.Recognition.EngineModel;

public sealed class EngineModelRecognition(ICommunicationChannel channel) : IEngineModelRecognition
{
    private readonly string _ctxKey = string.Intern("ENGINE_MODEL");
    private readonly string _ctxName = string.Intern("Двигатель");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

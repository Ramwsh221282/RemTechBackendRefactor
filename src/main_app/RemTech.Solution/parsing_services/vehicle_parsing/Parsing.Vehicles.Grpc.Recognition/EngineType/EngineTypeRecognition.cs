namespace Parsing.Vehicles.Grpc.Recognition.EngineType;

public sealed class EngineTypeRecognition(ICommunicationChannel channel) : IEngineTypeRecognition
{
    private readonly string _ctxKey = string.Intern("ENGINE_TYPE");
    private readonly string _ctxName = string.Intern("Тип двигателя");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

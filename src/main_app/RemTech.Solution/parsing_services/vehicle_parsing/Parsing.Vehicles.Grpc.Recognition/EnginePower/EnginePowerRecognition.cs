namespace Parsing.Vehicles.Grpc.Recognition.EnginePower;

public sealed class EnginePowerRecognition(ICommunicationChannel channel) : IEnginePowerRecognition
{
    private readonly string _ctxKey = string.Intern("ENGINE_POWER");
    private readonly string _ctxName = string.Intern("Мощность двигателя");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

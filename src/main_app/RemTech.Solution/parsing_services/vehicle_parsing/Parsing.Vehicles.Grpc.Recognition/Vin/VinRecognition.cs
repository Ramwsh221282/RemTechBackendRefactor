namespace Parsing.Vehicles.Grpc.Recognition.Vin;

public sealed class VinRecognition(ICommunicationChannel channel) : IVinRecognition
{
    private readonly string _ctxKey = string.Intern("VIN");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxKey
        );
    }
}

namespace Parsing.Vehicles.Grpc.Recognition.BuRecognition;

public sealed class BuRecognition(ICommunicationChannel channel) : IBuRecognition
{
    private readonly string _ctxKey = string.Intern("BU");
    private readonly string _ctxName = string.Intern("Б/у");

    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic characteristic = new RecognizedCharacteristic(
            await channel.Talker().Tell(text)
        ).ByKeyOrDefault(_ctxKey, _ctxName);
        return characteristic
            ? new Characteristic(_ctxName, string.Intern("Да"))
            : new Characteristic(_ctxName, string.Intern("Нет"));
    }
}

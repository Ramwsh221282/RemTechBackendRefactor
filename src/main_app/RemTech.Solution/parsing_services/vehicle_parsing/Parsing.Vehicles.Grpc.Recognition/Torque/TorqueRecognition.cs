namespace Parsing.Vehicles.Grpc.Recognition.Torque;

public sealed class TorqueRecognition(ICommunicationChannel channel) : ITorqueRecognition
{
    private readonly string _ctxKey = string.Intern("TORQUE");
    private readonly string _ctxName = string.Intern("Крутящий момент");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

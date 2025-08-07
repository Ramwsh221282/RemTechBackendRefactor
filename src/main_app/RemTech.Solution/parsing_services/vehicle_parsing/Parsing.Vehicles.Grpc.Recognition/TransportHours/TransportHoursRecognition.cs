namespace Parsing.Vehicles.Grpc.Recognition.TransportHours;

public sealed class TransportHoursRecognition(ICommunicationChannel channel)
    : ITransportHoursRecognition
{
    private readonly string _ctxKey = string.Intern("TRANSPORT_HOURS");
    private readonly string _ctxName = string.Intern("Моточасы");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

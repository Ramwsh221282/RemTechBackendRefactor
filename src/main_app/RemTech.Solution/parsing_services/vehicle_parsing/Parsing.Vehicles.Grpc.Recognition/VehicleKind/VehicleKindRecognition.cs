namespace Parsing.Vehicles.Grpc.Recognition.VehicleKind;

public sealed class VehicleKindRecognition(ICommunicationChannel channel) : IVehicleKindRecognition
{
    private readonly string _ctxKey = string.Intern("TRANSPORT_TYPE");
    private readonly string _ctxName = string.Intern("Тип техники");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

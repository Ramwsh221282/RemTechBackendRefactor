namespace Parsing.Vehicles.Grpc.Recognition.VehicleModel;

public sealed class VehicleModelRecognition(ICommunicationChannel channel)
    : IVehicleModelRecognition
{
    private readonly string _ctxKey = string.Intern("BRAND_MODEL");
    private readonly string _ctxName = string.Intern("Модель");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

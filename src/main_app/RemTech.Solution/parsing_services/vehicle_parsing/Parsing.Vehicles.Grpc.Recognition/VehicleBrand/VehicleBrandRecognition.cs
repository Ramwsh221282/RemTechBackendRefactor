namespace Parsing.Vehicles.Grpc.Recognition.VehicleBrand;

public sealed class VehicleBrandRecognition(ICommunicationChannel channel)
    : IVehicleBrandRecognition
{
    private readonly string _ctxKey = string.Intern("BRAND_NAME");
    private readonly string _ctxName = string.Intern("Марка");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}

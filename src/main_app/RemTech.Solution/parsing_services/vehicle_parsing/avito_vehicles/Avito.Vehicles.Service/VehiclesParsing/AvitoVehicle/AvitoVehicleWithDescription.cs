using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Description;
using Parsing.RabbitMq.PublishVehicle.Extras;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithDescription(
    IAvitoDescriptionSource source,
    IAvitoVehicle origin
) : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope origin1 = await origin.VehicleSource();
        string description = await source.Read();
        SentencesCollection sentences = new SentencesCollection();
        sentences.Fill(description);
        return new AvitoVehicleEnvelope(origin1, sentences);
    }
}

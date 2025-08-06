using System.Text;
using System.Text.Json;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;

namespace Avito.Vehicles.Service.VehiclesParsing;

public sealed class RabbitPublishingParsedVehicle(
    Serilog.ILogger logger,
    IPublishVehiclePublisher publisher
)
{
    public async Task SendAsync(
        IParsedVehicle vehicle,
        string parserName,
        string parserType,
        string parserDomain,
        string parserLinkName,
        string parserLinkUrl
    )
    {
        if (!await new ValidatingParsedVehicle(vehicle).IsValid())
        {
            logger.Warning("Vehicle was not sent to rabbit mq. Not valid.");
            return;
        }

        VehiclePublishMessage message = new VehiclePublishMessage(
            new ParserBody(parserName, parserType, parserDomain),
            new ParserLinkBody(parserName, parserType, parserDomain, parserLinkName, parserLinkUrl),
            await CreateVehicleBody(vehicle)
        );
        ReadOnlyMemory<byte> bytes = ParsedVehicleAsBytes(message);
        await publisher.Publish(message);
        logger.Information("Vehicle sended to rabbit mq.");
    }

    private static async Task<VehicleBody> CreateVehicleBody(IParsedVehicle vehicle)
    {
        ParsedVehiclePrice price = await vehicle.Price();
        CharacteristicsDictionary ctxes = await vehicle.Characteristics();
        UniqueParsedVehiclePhotos photos = await vehicle.Photos();
        ParsedVehicleGeo geo = await vehicle.Geo();
        return new VehicleBody(
            await vehicle.Identity(),
            await vehicle.Kind(),
            await vehicle.Brand(),
            await vehicle.Model(),
            price,
            price.IsNds(),
            geo.Region(),
            await vehicle.SourceUrl(),
            ctxes.Read().Select(c => new VehicleBodyCharacteristic(c.Name(), c.Value())),
            photos.Read().Select(p => new VehicleBodyPhoto(p))
        );
    }

    private static ReadOnlyMemory<byte> ParsedVehicleAsBytes(VehiclePublishMessage message)
    {
        string json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }
}

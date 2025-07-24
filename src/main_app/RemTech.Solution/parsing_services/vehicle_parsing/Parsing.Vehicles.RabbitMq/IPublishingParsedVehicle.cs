using Parsing.Vehicles.Common.Json;
using Parsing.Vehicles.Common.ParsedVehicles;

namespace Parsing.Vehicles.RabbitMq;

public interface IPublishingParsedVehicle
{
    Task<IParsedVehicle> Publish(ParsedVehicleParser parser, IParsedVehicle vehicle);
}
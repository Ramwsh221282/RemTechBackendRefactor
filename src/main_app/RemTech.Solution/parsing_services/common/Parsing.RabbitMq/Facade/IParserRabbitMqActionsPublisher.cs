using Parsing.RabbitMq.PublishSpare;
using Parsing.RabbitMq.PublishVehicle;

namespace Parsing.RabbitMq.Facade;

public interface IParserRabbitMqActionsPublisher
{
    Task SayParserFinished(string parserName, string parserType, long totalElapsedSeconds);

    Task SayParserLinkFinished(
        string parserName,
        string parserType,
        string linkName,
        long totalElapsedSeconds
    );

    Task SayVehicleFinished(VehiclePublishMessage message);

    Task SaySparePublished(SpareSinkMessage message);
}

using Parsing.RabbitMq.FinishParser;
using Parsing.RabbitMq.FinishParserLink;
using Parsing.RabbitMq.PublishVehicle;

namespace Parsing.RabbitMq.Facade;

internal sealed class ParserRabbitMqActionsPublisher(
    IParserFinishMessagePublisher finishParser,
    IParserLinkFinishedMessagePublisher finishLink,
    IPublishVehiclePublisher vehicle
) : IParserRabbitMqActionsPublisher
{
    public async Task SayParserFinished(
        string parserName,
        string parserType,
        long totalElapsedSeconds
    )
    {
        await finishParser.Publish(
            new ParserFinishedMessage(parserName, parserType, totalElapsedSeconds)
        );
    }

    public async Task SayParserLinkFinished(
        string parserName,
        string parserType,
        string linkName,
        long totalElapsedSeconds
    )
    {
        await finishLink.Publish(
            new FinishedParserLinkMessage(parserName, parserType, linkName, totalElapsedSeconds)
        );
    }

    public async Task SayVehicleFinished(VehiclePublishMessage message)
    {
        await vehicle.Publish(message);
    }
}

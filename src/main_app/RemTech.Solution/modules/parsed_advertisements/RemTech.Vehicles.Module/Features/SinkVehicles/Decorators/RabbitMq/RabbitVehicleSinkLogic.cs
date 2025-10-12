using RemTech.ContainedItems.Module.Features.MessageBus;
using Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.RabbitMq;

internal sealed class RabbitVehicleSinkLogic(
    IIncreaseProcessedPublisher publisher,
    IAddContainedItemsPublisher contained,
    Serilog.ILogger logger,
    ITransportAdvertisementSinking origin
) : ITransportAdvertisementSinking
{
    public async Task<Result.Pattern.Result> Sink(
        IVehicleJsonSink sink,
        CancellationToken ct = default
    )
    {
        Result.Pattern.Result result = await origin.Sink(sink, ct);
        if (result.IsSuccess)
        {
            string name = sink.ParserName();
            string type = sink.ParserType();
            string link = sink.LinkName();
            await publisher.SendIncreaseProcessed(name, type, link);
            await contained.Publish(
                new AddContainedItemMessage(
                    sink.VehicleId().Read(),
                    sink.ParserType(),
                    sink.SourceDomain(),
                    sink.SourceUrl()
                )
            );
            logger.Information(
                "Sended message to increase processed for {Parser} {Type} {Link}",
                name,
                type,
                link
            );
        }
        else
        {
            logger.Error("Error at saving vehicle: {Error}.", result.Error.ErrorText);
        }

        return result;
    }
}

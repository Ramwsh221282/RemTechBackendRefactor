using RemTech.Result.Library;
using Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.RabbitMq;

public sealed class RabbitVehicleSinkedSink(
    IIncreaseProcessedPublisher publisher,
    ITransportAdvertisementSinking origin,
    Serilog.ILogger logger
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        Status result = await origin.Sink(sink, ct);
        if (result.IsSuccess)
        {
            string name = sink.ParserName();
            string type = sink.ParserType();
            string link = sink.LinkName();
            await publisher.SendIncreaseProcessed(name, type, link);
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

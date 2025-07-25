using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.RabbitMq;

public sealed class RabbitVehicleSinkedSink : ITransportAdvertisementSinking
{
    private readonly ITransportAdvertisementSinking _origin;

    public RabbitVehicleSinkedSink(ITransportAdvertisementSinking origin) =>
        _origin = origin;
    
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        Status result = await _origin.Sink(sink, ct);
        if (result.IsSuccess)
        {
            Console.WriteLine("Should publish to parsers queue.");
        }

        return result;
    }
}
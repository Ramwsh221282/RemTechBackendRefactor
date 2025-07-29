using RemTech.RabbitMq.Adapter;

namespace RemTech.ParsedAdvertisements.Core.Features.SinkVehicles.Decorators.RabbitMq;

public sealed class RabbitVehicleSink : IDisposable, IAsyncDisposable
{
    private readonly RabbitMqChannel _channel;
    private readonly ITransportAdvertisementSinking _origin;
    private RabbitAcceptPoint? _acceptPoint;

    public RabbitVehicleSink(
        RabbitMqConnectionOptions options,
        ITransportAdvertisementSinking origin
    )
    {
        _channel = new RabbitMqChannel(options);
        _origin = origin;
    }

    public async Task OpenQueue(string queueName, CancellationToken ct = default)
    {
        _acceptPoint ??= await _channel.MakeAcceptPoint(
            queueName,
            async (_, @event) =>
            {
                using VehicleJsonSink sink = new VehicleJsonSink(@event.Body);
                await _origin.Sink(sink, ct);
                await _acceptPoint!.Acknowledge(@event, ct);
            }
        );
        await _acceptPoint.StartConsuming(ct);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _acceptPoint?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_acceptPoint != null)
            await _acceptPoint.DisposeAsync();
        await _channel.DisposeAsync();
    }
}

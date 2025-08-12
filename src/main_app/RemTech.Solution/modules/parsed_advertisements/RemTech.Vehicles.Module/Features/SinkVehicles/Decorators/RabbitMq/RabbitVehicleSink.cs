using System.Text.Json;
using RabbitMQ.Client;
using Shared.Infrastructure.Module.RabbitMq;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.RabbitMq;

internal sealed class RabbitVehicleSink : IDisposable, IAsyncDisposable
{
    private readonly RabbitMqChannel _channel;
    private readonly ITransportAdvertisementSinking _origin;
    private RabbitAcceptPoint? _acceptPoint;

    public RabbitVehicleSink(
        ConnectionFactory connectionFactory,
        ITransportAdvertisementSinking origin
    )
    {
        IConnection connection = connectionFactory.CreateConnectionAsync().Result;
        IChannel channel = connection.CreateChannelAsync().Result;
        _channel = new RabbitMqChannel(connection, channel);
        _origin = origin;
    }

    public async Task OpenQueue(CancellationToken ct = default)
    {
        _acceptPoint ??= await _channel.MakeAcceptPoint(
            async (_, @event) =>
            {
                try
                {
                    VehicleSinkBytes sink = new VehicleSinkBytes(@event.Body.ToArray());
                    await _origin.Sink(sink, ct);
                }
                finally
                {
                    await _channel.Acknowledge(@event, ct);
                }
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

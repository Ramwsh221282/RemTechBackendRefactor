using RemTech.Core.Shared.Exceptions;
using RemTech.Result.Library;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators;

public sealed class ExceptionHandlingVehicleSinking(ITransportAdvertisementSinking origin)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        try
        {
            return await origin.Sink(sink, ct);
        }
        catch (OperationException ex)
        {
            Error error = new Error(ex.Message, ErrorCodes.Conflict);
            return new Status(error);
        }
        catch (ValueNotValidException ex)
        {
            Error error = new Error(ex.Message, ErrorCodes.Conflict);
            return new Status(error);
        }
    }
}

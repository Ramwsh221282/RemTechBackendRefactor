using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Result;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators;

internal sealed class ExceptionHandlingVehicleSinking(
    Serilog.ILogger logger,
    ITransportAdvertisementSinking origin
) : ITransportAdvertisementSinking
{
    private const string Scope = nameof(ExceptionHandlingVehicleSinking);

    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        try
        {
            return await origin.Sink(sink, ct);
        }
        catch (OperationException ex)
        {
            LogException(ex);
            Error error = new Error(ex.Message, ErrorCodes.Conflict);
            return new Status(error);
        }
        catch (ValueNotValidException ex)
        {
            LogException(ex);
            Error error = new Error(ex.Message, ErrorCodes.Conflict);
            return new Status(error);
        }
        catch (Exception ex)
        {
            LogException(ex);
            return ErrorFromException(ex);
        }
    }

    private void LogException(Exception ex)
    {
        logger.Error("{Scope} error: {Ex}.", Scope, ex.Message);
    }

    private Status ErrorFromException(Exception ex)
    {
        return new Status(new Error(ex.Message, ErrorCodes.Conflict));
    }
}

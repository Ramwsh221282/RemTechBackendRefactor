using RemTech.Core.Shared.Exceptions;
using RemTech.Result.Pattern;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators;

internal sealed class ExceptionHandlingVehicleSinking(
    Serilog.ILogger logger,
    ITransportAdvertisementSinking origin
) : ITransportAdvertisementSinking
{
    private const string Scope = nameof(ExceptionHandlingVehicleSinking);

    public async Task<Result.Pattern.Result> Sink(
        IVehicleJsonSink sink,
        CancellationToken ct = default
    )
    {
        try
        {
            return await origin.Sink(sink, ct);
        }
        catch (OperationException ex)
        {
            LogException(ex);
            Error error = new Error(ex.Message, ErrorCodes.Conflict);
            return new Result.Pattern.Result(error);
        }
        catch (ValueNotValidException ex)
        {
            LogException(ex);
            Error error = new Error(ex.Message, ErrorCodes.Conflict);
            return new Result.Pattern.Result(error);
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

    private Result.Pattern.Result ErrorFromException(Exception ex)
    {
        return new Result.Pattern.Result(new Error(ex.Message, ErrorCodes.Conflict));
    }
}

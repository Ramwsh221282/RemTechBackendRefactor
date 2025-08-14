using Cleaners.Module.Domain;
using Cleaners.Module.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Cleaners.Module.Endpoints.Responses;

internal sealed class CleanerHttpResponseVeil(Func<Task<ICleaner>> cleanerFn)
    : ICleanerVeil,
        IResult
{
    private CleanerHttpResponseOutput _output = new(
        Guid.Empty,
        0,
        DateTime.MinValue,
        DateTime.MinValue,
        0,
        string.Empty,
        0,
        0,
        0,
        0
    );

    public void Accept(
        Guid id,
        int cleanedAmount,
        DateTime lastRun,
        DateTime nextRun,
        int waitDays,
        string state,
        int hours,
        int minutes,
        int seconds,
        int itemsDateDayThreshold
    )
    {
        _output = new CleanerHttpResponseOutput(
            id,
            cleanedAmount,
            lastRun,
            nextRun,
            waitDays,
            state,
            hours,
            minutes,
            seconds,
            itemsDateDayThreshold
        );
    }

    public ICleaner Behave() => throw new NotSupportedException("Use Execute async.");

    public Task<ICleaner> BehaveAsync(CancellationToken ct = default) =>
        throw new NotSupportedException("Use Execute async.");

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        try
        {
            ICleaner cleaner = await cleanerFn();
            cleaner.ProduceOutput().PrintTo(this);
            httpContext.Response.StatusCode = 200;
            await httpContext.Response.WriteAsJsonAsync(_output);
        }
        catch (CleanerIsAlreadyBusyException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (CleanerIsAlreadyStopedException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (CleanerIsAlreadyWaitingException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (CleanerIsNotBusyException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (CleanerIsBusyException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (CleanedAmountInvalidException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (CleanerDoesNotExistsException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (CleanerIdIsEmptyException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (InvalidWaitDaysException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (LastRunInvalidException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (NextRunInvalidException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (StateInvalidException ex)
        {
            await Results.BadRequest(new { message = ex.Message }).ExecuteAsync(httpContext);
        }
        catch (Exception)
        {
            await Results
                .InternalServerError(new { message = "Ошибка на стороне приложения." })
                .ExecuteAsync(httpContext);
        }
    }
}

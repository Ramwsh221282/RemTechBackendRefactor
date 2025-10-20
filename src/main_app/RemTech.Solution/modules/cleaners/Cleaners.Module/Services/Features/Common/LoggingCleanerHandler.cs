using Cleaners.Module.Domain;
using Cleaners.Module.Domain.Exceptions;
using Cleaners.Module.Services.Features.PermantlyEnable;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.Common;

internal sealed class LoggingCleanerHandler<TCommand>(
    Serilog.ILogger logger,
    ICommandHandler<TCommand, ICleaner> inner
) : ICommandHandler<TCommand, ICleaner>
    where TCommand : ICommand
{
    public async Task<ICleaner> Handle(TCommand command, CancellationToken ct = default)
    {
        try
        {
            ICleaner fromOrigin = await inner.Handle(command, ct);
            return fromOrigin.ProduceOutput().PrintTo(new LoggingCleanerVeil(logger)).Behave();
        }
        catch (CleanerIsAlreadyBusyException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanerIsAlreadyStopedException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanerIsAlreadyWaitingException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanerIsNotBusyException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanerIsBusyException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanedAmountInvalidException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanerDoesNotExistsException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanerIdIsEmptyException ex)
        {
            LogError(ex);
            throw;
        }
        catch (InvalidWaitDaysException ex)
        {
            LogError(ex);
            throw;
        }
        catch (LastRunInvalidException ex)
        {
            LogError(ex);
            throw;
        }
        catch (NextRunInvalidException ex)
        {
            LogError(ex);
            throw;
        }
        catch (StateInvalidException ex)
        {
            LogError(ex);
            throw;
        }
        catch (CleanerHasNoItemsToCleanException ex)
        {
            LogError(ex);
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    private void LogError(Exception ex)
    {
        logger.Error("{Ex}", ex.Message);
    }
}

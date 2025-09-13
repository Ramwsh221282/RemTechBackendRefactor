using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Features.UserPasswordRecovering.Core;
using Users.Module.Features.UserPasswordRecovering.Exceptions;

namespace Users.Module.Features.UserPasswordRecovering.Interactor;

internal sealed class UserPasswordExceptionLoggingNode
    : ICommandHandler<UserPasswordRecoveringContext>
{
    private readonly Serilog.ILogger _logger;
    private readonly ICommandHandler<UserPasswordRecoveringContext> _handler;

    public UserPasswordExceptionLoggingNode(
        Serilog.ILogger logger,
        ICommandHandler<UserPasswordRecoveringContext> handler
    )
    {
        _logger = logger;
        _handler = handler;
    }

    public async Task Handle(UserPasswordRecoveringContext command, CancellationToken ct = default)
    {
        try
        {
            await _handler.Handle(command, ct);
        }
        catch (UnableToDetermineHowToResetPasswordException ex)
        {
            LogErrorException(ex);
            throw;
        }
        catch (UserRecoveringPasswordByEmailInvalidException ex)
        {
            LogErrorException(ex);
            throw;
        }
        catch (UserRecoveringPasswordByLoginEmptyException ex)
        {
            LogErrorException(ex);
            throw;
        }
        catch (UserRecoveringPasswordKeyAlreadyExistsException ex)
        {
            LogErrorException(ex);
            throw;
        }
        catch (UserToRecoverNotFoundException ex)
        {
            LogErrorException(ex);
            throw;
        }
        catch (Exception ex)
        {
            LogFatalException(ex);
            throw;
        }
    }

    private void LogFatalException(Exception ex)
    {
        _logger.Error("{Ex}", ex);
    }

    private void LogErrorException(Exception ex)
    {
        _logger.Error("{Ex}", ex);
    }
}

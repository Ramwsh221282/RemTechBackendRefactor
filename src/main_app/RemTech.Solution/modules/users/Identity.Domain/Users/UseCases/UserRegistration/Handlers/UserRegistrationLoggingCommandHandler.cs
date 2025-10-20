using Identity.Domain.Users.UseCases.UserRegistration.Input;
using Identity.Domain.Users.UseCases.UserRegistration.Output;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration.Handlers;

public sealed class UserRegistrationLoggingCommandHandler(
    Serilog.ILogger logger,
    ICommandHandler<UserRegistrationCommand, Status<UserRegistrationResponse>> handler
) : ICommandHandler<UserRegistrationCommand, Status<UserRegistrationResponse>>
{
    private const string Context = nameof(UserRegistrationCommand);

    public async Task<Status<UserRegistrationResponse>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        Status<UserRegistrationResponse> result = await handler.Handle(command, ct);
        if (!result.IsFailure)
            return result;

        logger.Error("{Context}. Error: {ErrorText}.", Context, result.Error.ErrorText);
        return result.Error;
    }
}

using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration.Decorators;

public sealed class UserRegistrationLoggingCommandHandler(
    Serilog.ILogger logger,
    ICommandHandler<UserRegistrationCommand, Status<User>> handler
) : ICommandHandler<UserRegistrationCommand, Status<User>>
{
    private const string Context = nameof(UserRegistrationCommand);

    public async Task<Status<User>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        Status<User> result = await handler.Handle(command, ct);
        if (!result.IsFailure)
            return result;

        logger.Error("{Context}. Error: {ErrorText}.", Context, result.Error.ErrorText);
        return result.Error;
    }
}

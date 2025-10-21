using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmation.Decorators;

public sealed class CreateEmailConfirmationLoggingCommandHandler(
    ILogger logger,
    ICommandHandler<CreateEmailConfirmationCommand, Status<CreateEmailConfirmationResponse>> handler
) : ICommandHandler<CreateEmailConfirmationCommand, Status<CreateEmailConfirmationResponse>>
{
    private const string Context = nameof(CreateEmailConfirmationCommand);

    public async Task<Status<CreateEmailConfirmationResponse>> Handle(
        CreateEmailConfirmationCommand command,
        CancellationToken ct = default
    )
    {
        Status<CreateEmailConfirmationResponse> result = await handler.Handle(command, ct);
        if (result.IsFailure)
        {
            logger.Error("{Context}. Error: {ErrorMessage}.", Context, result.Error.ErrorText);
            return result;
        }

        logger.Information(
            "{Context}. Generated confirmation ticket for user id: {Id}.",
            Context,
            command.UserId
        );
        return result;
    }
}

using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Identity.Domain.Users.UseCases.SignOut.Handlers;

public sealed class SignOutLoggingComandHandler(
    ILogger logger,
    ICommandHandler<SignOutCommand, Status> handler
) : ICommandHandler<SignOutCommand, Status>
{
    private const string Context = nameof(SignOutCommand);

    public async Task<Status> Handle(SignOutCommand command, CancellationToken ct = default)
    {
        Status result = await handler.Handle(command, ct);

        if (result.IsFailure)
        {
            logger.Error("{Context}. Error: {ErrorText}.", Context, result.Error.ErrorText);
            return result;
        }

        logger.Information(
            "{Context}. Token: {TokenId} has been signed out.",
            Context,
            command.TokenId
        );
        return result;
    }
}

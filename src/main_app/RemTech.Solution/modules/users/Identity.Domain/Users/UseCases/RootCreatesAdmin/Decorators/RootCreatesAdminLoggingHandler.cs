using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Identity.Domain.Users.UseCases.RootCreatesAdmin.Decorators;

public sealed class RootCreatesAdminLoggingHandler(
    ILogger logger,
    ICommandHandler<RootCreatesAdminCommand, Status<User>> handler
) : ICommandHandler<RootCreatesAdminCommand, Status<User>>
{
    private const string Context = nameof(RootCreatesAdminCommand);

    public async Task<Status<User>> Handle(
        RootCreatesAdminCommand command,
        CancellationToken ct = default
    )
    {
        Status<User> created = await handler.Handle(command, ct);
        if (created.IsFailure)
        {
            logger.Error("{Context}. Error: {ErrorMessage}.", Context, created.Error.ErrorText);
            return created;
        }

        User createdModel = created.Value;

        logger.Information("{Context}. Created user by admin.", Context);
        logger.Information(
            """
            Creation information:
            Root ID: {RootId}
            User ID: {UserId}
            User Email: {Email}
            User Login: {Login}
            """,
            command.CreatorId,
            createdModel.Id.Id,
            createdModel.Email.Email,
            createdModel.Login.Name
        );

        return created;
    }
}

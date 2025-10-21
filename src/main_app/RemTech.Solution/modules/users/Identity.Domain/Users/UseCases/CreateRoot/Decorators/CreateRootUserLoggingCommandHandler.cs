using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Serilog;

namespace Identity.Domain.Users.UseCases.CreateRoot.Decorators;

public sealed class CreateRootUserLoggingCommandHandler(
    ILogger logger,
    ICommandHandler<CreateRootUserCommand, Status<User>> handler
) : ICommandHandler<CreateRootUserCommand, Status<User>>
{
    private const string Context = nameof(CreateRootUserCommand);

    public async Task<Status<User>> Handle(
        CreateRootUserCommand command,
        CancellationToken ct = default
    )
    {
        Status<User> root = await handler.Handle(command, ct);
        if (root.IsFailure)
        {
            logger.Error("{Context}. Error: {ErrorMessage}.", Context, root.Error.ErrorText);
            return root;
        }

        User rootModel = root.Value;

        logger.Information(
            """
            {Context}. Created root user account:
            ID: {Id}
            Login: {Login}
            Email: {Email}
            """,
            Context,
            rootModel.Id.Id,
            rootModel.Login.Name,
            rootModel.Email.Email
        );

        return rootModel;
    }
}

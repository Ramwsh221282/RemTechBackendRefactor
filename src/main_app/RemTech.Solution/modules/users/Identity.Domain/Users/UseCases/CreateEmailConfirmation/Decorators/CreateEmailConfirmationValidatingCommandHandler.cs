using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmation.Decorators;

public sealed class CreateEmailConfirmationValidatingCommandHandler(
    ICommandHandler<CreateEmailConfirmationCommand, Status<CreateEmailConfirmationResponse>> handler
) : ICommandHandler<CreateEmailConfirmationCommand, Status<CreateEmailConfirmationResponse>>
{
    public async Task<Status<CreateEmailConfirmationResponse>> Handle(
        CreateEmailConfirmationCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope().Check(UserId.Create(command.UserId));
        return scope.Any() ? scope.ToError() : await handler.Handle(command, ct);
    }
}

using Shared.Infrastructure.Module.Cqrs;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreatingNewAccount.Exceptions;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserByAdminValidatorWrapper(
    ICommandHandler<AddUserByAdminCommand, AddUserByAdminResult> origin
) : ICommandHandler<AddUserByAdminCommand, AddUserByAdminResult>
{
    public Task<AddUserByAdminResult> Handle(
        AddUserByAdminCommand command,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new UserRegistrationRequiresNameException();
        if (string.IsNullOrWhiteSpace(command.Email))
            throw new UserRegistrationRequiresEmailException();
        new EmailValidation().ValidateEmail(command.Email);
        return origin.Handle(command, ct);
    }
}

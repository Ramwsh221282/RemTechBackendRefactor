using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Users.Module.CommonAbstractions;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserByAdminValidatorWrapper(
    ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>> origin
) : ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>>
{
    public async Task<Status<AddUserByAdminResult>> Handle(
        AddUserByAdminCommand command,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Error.Validation(
                "Для создания учетной записи нужно указать название учетной записи."
            );

        if (string.IsNullOrWhiteSpace(command.Email))
            return Error.Validation("Для создания учетной записи нужно указать почту");

        new EmailValidation().ValidateEmail(command.Email);
        return await origin.Handle(command, ct);
    }
}

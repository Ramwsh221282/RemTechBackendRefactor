using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Ports.EventHandlers;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Roles.UseCases.AddNewRole;

public sealed class AddRoleHandler(
    IIdentityUserEventHandler eventHandler,
    IValidator<AddRoleCommand> validator
) : ICommandHandler<AddRoleCommand, Status<Role>>
{
    public async Task<Status<Role>> Handle(AddRoleCommand command, CancellationToken ct = default)
    {
        ValidationResult? validation = await validator.ValidateAsync(command, ct);
        if (validation.IsValid == false)
            return validation.ValidationError();

        RoleName name = RoleName.Create(command.RoleName);
        Role role = Role.Create(name);

        Status processing = await role.PublishEvents(eventHandler, ct);
        return processing.IsFailure ? processing.Error : role;
    }
}
